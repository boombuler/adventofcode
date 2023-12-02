namespace AdventOfCode.Utils.Parser;

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

delegate Result<(T Value, Input RemainingInput)> ParserFn<T>(Input input);


class Parser<T>
{
    private readonly ParserFn<T> fFn;

    private Parser(ParserFn<T> fn)
    {
        fFn = fn;
    }

    public static implicit operator Parser<T>(ParserFn<T> fn)
        => new Parser<T>(fn);

    public static Parser<T> operator |(Parser<T> one, Parser<T> two)
    {
        return one.Or(two);
    }

    public Result<(T Value, Input RemainingInput)> Parse(Input input)
        => fFn(input);

    public Parser<T> Opt(T fallback = default)
        => new Parser<T>((input) =>
        {
            var res = fFn(input);
            if (!res.HasValue)
                return (fallback, input);
            return res.Value;
        });

    public Parser<TResult> Map<TResult>(Func<T, TResult> map)
        => new Parser<TResult>((input) => fFn(input).Map(r => (map(r.Value), r.RemainingInput)));

    public Parser<(T, TOther)> Then<TOther>(Parser<TOther> other)
        => new Parser<(T, TOther)>((input) =>
        {
            var res1 = fFn(input);
            if (!res1.HasValue)
                return res1.Error;

            var res2 = other.Parse(res1.Value.RemainingInput);
            if (!res2.HasValue)
                return res2.Error;

            return ((res1.Value.Value, res2.Value.Value), res2.Value.RemainingInput);
        });

    public Parser<TResult> Then<TOther, TResult>(Parser<TOther> other, Func<T, TOther, TResult> map)
        => Then(other).Map(v => map(v.Item1, v.Item2));

    public Parser<T> ThenL<TOther>(Parser<TOther> other)
        => Then(other, (l, _) => l);

    public Parser<TOther> ThenR<TOther>(Parser<TOther> other)
        => Then(other, (_, r) => r);

    public static implicit operator Parser<T>(T s)
        => Parser.Str(s.ToString()).Map(_ => s);

    public T MustParse(string s)
        => Parse(new Input(s)).Value.Value;

    public Parser<T> Assert(Predicate<T> condition, string error)
        => new Parser<T>((input) =>
        {
            var res = fFn(input);
            if (!res.HasValue)
                return res.Error;

            if (!condition(res.Value.Value))
                return error;
            return res.Value;
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
                result.Add(res.Value.Value);
                input = res.Value.RemainingInput;
            }
            return (result.ToArray(), input);
        });

    public Parser<T[]> Many1()
        => Many().Assert(itms => itms.Any(), "Expected at least one item");

    public Parser<T> Or(Parser<T> other)
        => new Parser<T>((input) =>
        {
            var r1 = fFn(input);
            if (r1.HasValue)
                return r1.Value;
            return other.Parse(input);
        });
}

static class Parser 
{ 
    static Parser<char> Expect(Func<char, bool> predicate)
        => new ParserFn<char>((input) =>
        {
            if (input.EOF)
                return Result<(char, Input)>.Failed("EOF not expected");
            if (predicate(input.Current))
                return Result<(char, Input)>.Success((input.Current, input.Next()));
            return Result<(char, Input)>.Failed("Unexpected character");
        });
    
    public static Parser<char> Char(char input)
        => Expect(c => c == input);

    public static Parser<char> AnyChar(params char[] input)
        => Expect(SearchValues.Create(input).Contains);

    public static Parser<TValue[]> List<TValue>(this Parser<TValue> parser, params char[] separators)
        => AnyChar(separators).Many().ThenR(parser).Many();

    private static Parser<TValue[]> Sequence<TValue>(Parser<TValue>[] parsers)
        => new ParserFn<TValue[]>((input) =>
        {
            var values = new TValue[parsers.Length];
            for (int i = 0; i < parsers.Length; i++)
            {
                var res = parsers[i].Parse(input);
                if (!res.HasValue) 
                    return res.Error;
                values[i] = res.Value.Value;
                input = res.Value.RemainingInput;
            }
            return (values, input);
        });

    public static Parser<string> Str(string s)
        => Sequence(s.Select(Char).ToArray()).Text();

    public static Parser<string> Text(this Parser<char[]> parser)
        => parser.Map(chars => new string(chars));

    public static Parser<char> Digit => Expect(char.IsAsciiDigit);
    public static Parser<string> Digits => Digit.Many1().Text();
    public static Parser<int> Int
        => (Char('-').Map(_ => -1).Opt(1))
            .Then(Digits, (sign, digits) => sign * int.Parse(digits));

    public static Parser<T> Enum<T>() where T : struct, System.Enum
        => System.Enum.GetValues<T>()
            .Select(e => Str(e.ToString()).Map(_ => e))
            .Aggregate((a, b) => a | b);

    public static Parser<T> Regex<T>([StringSyntax("regex")] string pattern)
    {
        var regex = new Regex("^" + pattern, RegexOptions.Compiled);
        var factory = regex.CreateMatchFactory<T>();
        if (factory == null)
            throw new InvalidOperationException();
        return new ParserFn<T>((input) =>
        {
            var match = regex.Match(input.Remaining);
            if (!match.Success)
                return "Failed to match regex";

            input = input.Seek(match.Length);
            return (factory(match), input);
        });
    }

}
