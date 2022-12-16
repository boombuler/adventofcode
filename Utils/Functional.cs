namespace AdventOfCode.Utils;

static class Functional
{
    public static T Identity<T>(T e) => e;

    public static Func<T1, T3> Combine<T1, T2, T3>(this Func<T1, T2> first, Func<T2, T3> second)
        => (T1 v) => second(first(v));


    public static Func<TIn, TOut> Memorize<TIn, TOut>(this Func<TIn, TOut> fn)
    {
        var cache = new Dictionary<TIn, TOut>();
        return (arg) =>
        {
            if (cache.TryGetValue(arg, out var result))
                return result;
            return cache[arg] = fn(arg);
        };
    }
    public static Func<TIn1, TIn2, TOut> Memorize<TIn1, TIn2, TOut>(this Func<TIn1, TIn2, TOut> fn)
    {
        var m = Memorize<(TIn1 a, TIn2 b), TOut>(a => fn(a.a, a.b));
        return (a, b) => m((a, b));
    }

    public static Func<TIn1, TIn2, TIn3, TOut> Memorize<TIn1, TIn2, TIn3, TOut>(this Func<TIn1, TIn2, TIn3, TOut> fn)
    {
        var m = Memorize<(TIn1 a, TIn2 b, TIn3 c), TOut>(a => fn(a.a, a.b, a.c));
        return (a, b, c) => m((a, b, c));
    }
}
