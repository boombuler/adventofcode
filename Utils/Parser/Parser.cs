﻿namespace AdventOfCode.Utils.Parser;

using System.Buffers;
using System.Diagnostics.CodeAnalysis;

static class Parser
{
    public static Parser<char> Any { get; } = Expect(_ => true);

    public static Parser<char> NL { get; } = Char('\n');

    public static Parser<char> WS { get; } = AnyChar(' ', '\t');

    static Parser<char> Expect(Func<char, bool> predicate)
        => new Parser<char>((input) =>
        {
            if (input.EOF)
                return "EOF not expected";
            if (predicate(input.Current))
                return new Result<char>(input.Current, input.Next());
            return "Unexpected character";
        });
    
    public static Parser<char> Char(char input)
        => Expect(c => c == input);

    public static Parser<char> AnyChar(params char[] input)
        => Expect(SearchValues.Create(input).Contains);

    public static Parser<char> CharExcept(params char[] input)
    {
        var values = SearchValues.Create(input);
        return Expect(c => !values.Contains(c));
    }

    private static Parser<TValue[]> Sequence<TValue>(Parser<TValue>[] parsers)
        => new Parser<TValue[]>((input) =>
        {
            var values = new TValue[parsers.Length];
            for (int i = 0; i < parsers.Length; i++)
            {
                var res = parsers[i].Parse(input);
                if (!res.HasValue) 
                    return res.Error;
                values[i] = res.Value;
                input = res.Input;
            }
            return new Result<TValue[]>(values, input);
        });


    public static Parser<string> Str(string s)
        => Sequence(s.Select(Char).ToArray()).Text();

    public static Parser<string> Text(this Parser<char[]> parser)
        => parser.Select(chars => new string(chars));

    public static Parser<char> Digit => Expect(char.IsAsciiDigit);

    public static Parser<string> Digits => Digit.Many1().Text();

    public static Parser<long> Int
        => (Char('-').Select(_ => -1).Opt(1))
            .Then(Digits, (sign, digits) => sign * long.Parse(digits));

    public static Parser<T> Enum<T>() where T : struct, Enum
        => System.Enum.GetValues<T>()
            .Select(e => Str(e.ToString()).Select(_ => e))
            .Aggregate((a, b) => a.Or(b));

    public static Parser<T> Regex<T>([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        var regex = new Regex("^" + pattern, RegexOptions.Compiled);
        var factory = regex.CreateMatchFactory<T>();
        if (factory == null)
            throw new InvalidOperationException();
        return new Parser<T>((input) =>
        {
            var match = regex.Match(input.Remaining);
            if (!match.Success)
                return "Failed to match regex";

            input = input.Seek(match.Length);
            return new Result<T>(factory(match), input);
        });
    }
}
