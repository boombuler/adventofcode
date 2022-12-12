﻿namespace AdventOfCode.Utils;

using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public static class RegexHelper
{
    public static bool TryMatch<T>(this Regex self, string value, out T match)
    {
        bool res;
        (res, match) = self.ToFactoryImpl<T>()(value);
        return res;
    }

    public static Func<string, T> ToFactory<T>(this Regex self)
    {
        Func<string, (bool, T)> f = self.ToFactoryImpl<T>();
        return (s) => f(s).Item2;
    }

    private static Func<Match, (bool, object)> CreateBinder(string groupName, Type targetType)
    {

        var elementType = targetType.IsArray ? targetType.GetElementType() : targetType;

        Func<string, object> convert = (s) => Convert.ChangeType(s, elementType);
        if (elementType.IsEnum)
            convert = (s) => Enum.Parse(elementType, s);

        Func<Group, object> convertGroup = (g) => convert(g.Value);
        if (targetType.IsArray)
        {
            convertGroup = (g) =>
            {
                var res = Array.CreateInstance(elementType, g.Captures.Count);
                for (int i = 0; i < g.Captures.Count; i++)
                    res.SetValue(convert(g.Captures[i].Value), i);
                return res;
            };
        }
        var fallback = targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        return (m) =>
        {
            var group = m.Groups[groupName];
            if (group.Success)
                return (true, convertGroup(group));
            else
                return (false, fallback);
        };
    }

    private static Action<T, Match> CreatePropertyFieldBinder<T>(string name)
    {
        const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        var fld = typeof(T).GetField(name, FLAGS);
        var prp = fld != null ? null : typeof(T).GetProperty(name, FLAGS);
        if (fld == null && prp == null)
            return null;

        var binder = CreateBinder(name, fld?.FieldType ?? prp.PropertyType);

        return (T target, Match m) =>
        {
            (var ok, var value) = binder(m);
            if (ok)
            {
                if (fld != null)
                    fld.SetValue(target, value);
                else 
                    prp?.SetValue(target, value);
            }
        };
    }

    private static bool TryCreateConstructorFactory<T>(Regex regex, out Func<string, (bool, T)> factory)
    {
        var groupNames = regex.GetGroupNames().Where(gn => !int.TryParse(gn, out int tmp)).OrderBy(n => n).ToList();
        var constructor =
            typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance)
                .FirstOrDefault(ctor =>
                    ctor
                        .GetParameters()
                        .Select(p => p.Name)
                        .OrderBy(n => n)
                        .SequenceEqual(groupNames, StringComparer.OrdinalIgnoreCase)
                );
        factory = null;
        if (constructor == null)
            return false;

        Func<Match, object> BinderOrDefault(ParameterInfo arg)
        {
            var name = groupNames.First(gn => StringComparer.OrdinalIgnoreCase.Equals(gn, arg.Name));
            var binder = CreateBinder(name, arg.ParameterType);
            return (match) => binder(match).Item2;
        }

        Func<Match, object>[] arguments = constructor.GetParameters().Select(BinderOrDefault).ToArray();

        factory = (s) =>
        {
            var match = regex.Match(s);
            if (!match.Success)
                return (false, default(T));
            var args = arguments.Select(a => a(match)).ToArray();
            return (true, (T)constructor.Invoke(args));
        };

        return true;
    }

    private static Func<string, (bool, T)> ToFactoryImpl<T>(this Regex self)
    {
        if (TryCreateConstructorFactory<T>(self, out var factory))
            return factory;

        var setters = self.GetGroupNames().Select(CreatePropertyFieldBinder<T>).Where(f => f != null).ToList();
        if (setters.Count == 0)
            throw new InvalidOperationException();

        return (string s) =>
        {
            var m = self.Match(s);
            if (!m.Success)
                return (false, default(T));

            var result = Activator.CreateInstance<T>();
            foreach (var set in setters)
                set(result, m);
            return (true, result);
        };
    }
}
