using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventHelper
{
    public static class RegexHelper
    {
        public static bool TryMatch<T>(this Regex self, string value, out T match)
            where T : new()
        {
            bool res;
            (match, res) = self.ToFactoryImpl<T>()(value);
            return res;
        }

        public static Func<string, T> ToFactory<T>(this Regex self)
            where T : new()
        {
            Func<string, (T, bool)> f = self.ToFactoryImpl<T>();
            return (s) => f(s).Item1;
        }

        private static Func<string, (T, bool)> ToFactoryImpl<T>(this Regex self)
            where T : new()
        {
            const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var setters = self.GetGroupNames().Select<string, Action<T, Match>>(grp =>
            {
                var fld = typeof(T).GetField(grp, FLAGS);
                var prp = fld != null ? null : typeof(T).GetProperty(grp, FLAGS);
                if (fld == null && prp == null)
                    return null;

                
                var targetType = fld?.FieldType ?? prp.PropertyType;
                Func<string, object> convert = (s) => Convert.ChangeType(s, targetType);
                if (targetType.IsEnum)
                    convert = (s) => Enum.Parse(targetType, s);
                return (T target, Match m) =>
                {
                    var g = m.Groups[grp];
                    if (g.Success)
                    {
                        var value = convert(g.Value);
                        if (fld != null)
                            fld.SetValue(target, value);
                        else if (prp != null)
                            prp.SetValue(target, value);
                    }
                };
            }).Where(f => f != null).ToList();
            if (setters.Count == 0)
                throw new InvalidOperationException();

            return (string s) =>
            {
                var m = self.Match(s);
                if (!m.Success)
                    return (default(T), false);

                var result = new T();
                foreach (var set in setters)
                    set(result, m);
                return (result, true);
            };
        }
    }
}
