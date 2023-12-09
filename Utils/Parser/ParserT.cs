namespace AdventOfCode.Utils.Parser;
sealed class Parser<T>(Parser<T>.ParseDelegate fn)
{
    public delegate Result<T> ParseDelegate(Input input);

    public Result<T> Parse(Input input)
        => fn(input);

    public Parser<T> Opt(T fallback = default)
        => new((input) =>
        {
            var res = fn(input);
            if (!res.HasValue)
                return new Result<T>(fallback, input);
            return res;
        });

    public Parser<T> Token()
        => Parser.WS.Many().ThenR(this).ThenL(Parser.WS.Many());

    public Parser<(T, TOther)> Then<TOther>(Parser<TOther> other)
        => new((input) =>
        {
            var res1 = fn(input);
            if (!res1.HasValue)
                return res1.Error;

            var res2 = other.Parse(res1.Input);
            if (!res2.HasValue)
                return res2.Error;

            return new Result<(T, TOther)>((res1.Value, res2.Value), res2.Input);
        });

    public Parser<TResult> Then<TOther, TResult>(Parser<TOther> other, Func<T, TOther, TResult> map)
        => Then(other).Select(v => map(v.Item1, v.Item2));

    public Parser<T> ThenL<TOther>(Parser<TOther> other)
        => Then(other, (l, _) => l);
 
    public Parser<TOther> ThenR<TOther>(Parser<TOther> other)
        => Then(other, (_, r) => r);

    public Parser<TResult> Select<TResult>(Func<T, TResult> map)
        => new((input) => fn(input).Map(map));

    public Parser<TResult> Return<TResult>(TResult value)
        => new((input) => fn(input).Map(_ => value));

    public Parser<R> SelectMany<U, R>(Func<T, Parser<U>> selector, Func<T, U, R> result)
        => new((input) =>
        {
            var curRes = fn(input);
            if (!curRes.HasValue)
                return curRes.Error;

            var nextRes = selector(curRes.Value).Parse(curRes.Input);
            if (!nextRes.HasValue)
                return nextRes.Error;

            var res = result(curRes.Value, nextRes.Value);
            return new Result<R>(res, nextRes.Input);
        });

    public Parser<T> Where(Func<T, bool> predicate)
        => Assert(predicate, "Filter failed");

    public T MustParse(string s)
        => Parse(new Input(s)).Value;

    public Parser<T> Assert(Func<T, bool> condition, string error)
        => new((input) =>
        {
            var res = fn(input);
            if (res.HasValue && !condition(res.Value))
                return error;
            return res;
        });

    public Parser<T[]> Many()
        => new((input) =>
        {
            var result = new List<T>();
            while (true)
            {
                var res = fn(input);
                if (!res.HasValue)
                    break;
                result.Add(res.Value);
                input = res.Input;
            }
            return new Result<T[]>([.. result], input);
        });

    public Parser<T[]> Many1()
        => Many().Assert(itms => itms.Length > 0, "Expected at least one item");

    public Parser<T[]> Take(int count)
        => new(input =>
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                var res = fn(input);
                if (!res.HasValue)
                    return res.Error;
                result[i] = res.Value;
                input = res.Input;
            }
            return new Result<T[]>(result, input);
        });

    public Parser<T> Or(Parser<T> other)
        => new((input) =>
        {
            var r1 = fn(input);
            if (r1.HasValue)
                return r1;
            return other.Parse(input);
        });

    public Parser<T> Except<U>(Parser<U> except)
        => new((input) =>
        {
            if (except.Parse(input).HasValue)
                return "Except!";
            return fn(input);
        });

    public Parser<T[]> Until(string s)
       => Until(Parser.Str(s));

    public Parser<T[]> Until<U>(Parser<U> until)
        => Except(until).Many().ThenL(until);

    public Parser<T[]> List<U>(Parser<U> separator)
        => this.Then(separator.ThenR(this).Many(), (first, cons) => cons.Prepend(first).ToArray()).Opt([]);

    public Parser<T[]> List(params char[] separators)
        => List(Parser.AnyChar(separators));

    public static Parser<T> operator +(string s, Parser<T> p)
        => Parser.Str(s).ThenR(p);

    public static Parser<T> operator +(Parser<T> p, string s)
        => p.ThenL(Parser.Str(s));

    public static implicit operator Func<string, T>(Parser<T> p)
        => p.MustParse;

    public static Parser<T> operator |(Parser<T> a, Parser<T> b)
        => a.Or(b);
}