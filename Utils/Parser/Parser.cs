namespace AdventOfCode.Utils.Parser;

using System.Buffers;
using System.Diagnostics.CodeAnalysis;

class Parser<T>
{
    public delegate Result<T> ParseDelegate(Input input);

    private readonly ParseDelegate fFn;

    public Parser(ParseDelegate fn)
    {
        fFn = fn;
    }

    public static Parser<T> operator |(Parser<T> one, Parser<T> two)
        => one.Or(two);

    public Result<T> Parse(Input input)
        => fFn(input);

    public Parser<T> Opt(T fallback = default)
        => new Parser<T>((input) =>
        {
            var res = fFn(input);
            if (!res.HasValue)
                return new Result<T>(fallback, input);
            return res;
        });

    public Parser<TResult> Map<TResult>(Func<T, TResult> map)
        => new Parser<TResult>((input) => fFn(input).Map(map));

    public Parser<(T, TOther)> Then<TOther>(Parser<TOther> other)
        => new Parser<(T, TOther)>((input) =>
        {
            var res1 = fFn(input);
            if (!res1.HasValue)
                return res1.Error;

            var res2 = other.Parse(res1.Input);
            if (!res2.HasValue)
                return res2.Error;

            return new Result<(T, TOther)>((res1.Value, res2.Value), res2.Input);
        });

    public Parser<TResult> Then<TOther, TResult>(Parser<TOther> other, Func<T, TOther, TResult> map)
        => Then(other).Map(v => map(v.Item1, v.Item2));

    public Parser<T> ThenL<TOther>(Parser<TOther> other)
        => Then(other, (l, _) => l);

    public Parser<TOther> ThenR<TOther>(Parser<TOther> other)
        => Then(other, (_, r) => r);

    public T MustParse(string s)
        => Parse(new Input(s)).Value;

    public Parser<T> Assert(Func<T, bool> condition, string error)
        => new Parser<T>((input) =>
        {
            var res = fFn(input);
            if (res.HasValue && !condition(res.Value))
                return error;
            return res;
        });

    public Parser<T[]> Many()
        => new Parser<T[]>((input) =>
        {
            var result = new List<T>();
            while (true)
            {
                var res = fFn(input);
                if (!res.HasValue)
                    break;
                result.Add(res.Value);
                input = res.Input;
            }
            return new Result<T[]>(result.ToArray(), input);
        });

    public Parser<T[]> Many1()
        => Many().Assert(itms => itms.Any(), "Expected at least one item");

    public Parser<T> Or(Parser<T> other)
        => new Parser<T>((input) =>
        {
            var r1 = fFn(input);
            if (r1.HasValue)
                return r1;
            return other.Parse(input);
        });

    public static Parser<T> operator +(string s, Parser<T> p)
        => Parser.Str(s).ThenR(p);
    public static Parser<T> operator +(Parser<T> p, string s)
        => p.ThenL(Parser.Str(s));

    public Parser<R> SelectMany<U, R>(Func<T, Parser<U>> selector, Func<T, U, R> result)
        => new Parser<R>((input) =>
        {
            var curRes = fFn(input);
            if (!curRes.HasValue)
                return curRes.Error;

            var nextRes = selector(curRes.Value).Parse(curRes.Input);
            if (!nextRes.HasValue)
                return nextRes.Error;

            var res = result(curRes.Value, nextRes.Value);
            return new Result<R>(res, nextRes.Input);
        });
    public Parser<R> Select<R>(Func<T, R> selector)
        => Map(selector);
    public Parser<T> Where(Func<T, bool> predicate)
        => Assert(predicate, "Filter failed");

    public Parser<T> Except<U>(Parser<U> except)
        => new Parser<T>((input) =>
        {
            if (except.Parse(input).HasValue)
                return "Except!";
            return fFn(input);
        });
}
static class Parser
{ 
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

    public static Parser<T[]> Until<T>(this Parser<T> parser, string s)
        => Until(parser, Str(s));
    public static Parser<T[]> Until<T, U>(this Parser<T> parser, Parser<U> until)
        => parser.Except(until).Many().ThenL(until);

    public static Parser<TValue[]> List<TValue>(this Parser<TValue> parser, params char[] separators)
        => AnyChar(separators).Many().ThenR(parser).Many();

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

    public static Parser<char> Any => Expect(_ => true);
    public static Parser<string> Str(string s)
        => Sequence(s.Select(Char).ToArray()).Text();

    public static Parser<string> Text(this Parser<char[]> parser)
        => parser.Map(chars => new string(chars));

    public static Parser<char> Digit => Expect(char.IsAsciiDigit);
    public static Parser<string> Digits => Digit.Many1().Text();
    public static Parser<int> Int
        => (Char('-').Map(_ => -1).Opt(1))
            .Then(Digits, (sign, digits) => sign * int.Parse(digits));

    public static Parser<T> Token<T>(this Parser<T> t)
        => Char(' ').Many().ThenR(t).ThenL(Char(' ').Many());

    public static Parser<T> Enum<T>() where T : struct, Enum
        => System.Enum.GetValues<T>()
            .Select(e => Str(e.ToString()).Map(_ => e))
            .Aggregate((a, b) => a | b);

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
