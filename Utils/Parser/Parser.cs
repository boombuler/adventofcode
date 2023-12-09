﻿namespace AdventOfCode.Utils.Parser;

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

static class Parser
{
    public static Parser<char> Any { get; } = Expect(_ => true);

    public static Parser<char> NL { get; } = Char('\n');

    public static Parser<char> WS { get; } = AnyChar([' ', '\t']);

    public static Parser<string> Word { get; } = Expect(char.IsAsciiLetter).Many1().Text();

    static Parser<char> Expect(Func<char, bool> predicate)
        => new((input) =>
        {
            if (input.EOF)
                return "EOF not expected";
            if (predicate(input.Current))
                return new Result<char>(input.Current, input.Next());
            return "Unexpected character";
        });
    
    public static Parser<char> Char(char input)
        => Expect(c => c == input);
    public static Parser<T> Char<T>(char input, T value)
        => Expect(c => c == input).Return(value);

    public static Parser<char> AnyChar(ReadOnlySpan<char> input)
        => Expect(SearchValues.Create(input).Contains);

    public static Parser<char> CharExcept(params char[] input)
    {
        var values = SearchValues.Create(input);
        return Expect(c => !values.Contains(c));
    }

    private static Parser<TValue[]> Sequence<TValue>(Parser<TValue>[] parsers)
        => new((input) =>
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
        => new(input =>
        {
            foreach(var c in s)
            {
                if (input.EOF)
                    return "Unexpected end of input";
                if (input.Current != c)
                    return "Unexpected character";
                input = input.Next();
            }
            return new Result<string>(s, input);
        });

    public static Parser<T> Str<T>(string s, T value)
        => Str(s).Return(value);

    public static Parser<string> Text(this Parser<char[]> parser)
        => parser.Select(chars => new string(chars));

    public static Parser<char> Letter => Expect(char.IsAsciiLetter);
    public static Parser<char> Digit => Expect(char.IsAsciiDigit);

    public static Parser<string> Digits => Digit.Many1().Text();

    public static Parser<long> Int
        => (Char('-').Return(-1).Opt(1))
            .Then(Digits, (sign, digits) => sign * long.Parse(digits));

    public static Parser<BigInteger> BigInt
        => from sign in (Char('-').Return(-1).Opt(1))
           from digits in Digit.Many1().Select(d => d.Select(n => new BigInteger(n - '0')).Aggregate((a, b) => (a * 10) + b))
           select sign * digits;

    public static Parser<T> Enum<T>() where T : struct, Enum
        => System.Enum.GetValues<T>()
            .Select(e => Str(e.ToString()).Return(e))
            .Aggregate((a, b) => a.Or(b));

    public static Parser<T> Regex<T>([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        var regex = new Regex("^" + pattern, RegexOptions.Compiled);
        var factory = regex.CreateMatchFactory<T>() ?? throw new InvalidOperationException(); 
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