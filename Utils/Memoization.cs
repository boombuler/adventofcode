namespace AdventOfCode.Utils;

static class Memoization
{
    public static Func<A, B, R> Recursive<A, B, R>(Func<A, B, Func<A, B, R>, R> memorizableFn)
    {
        var fn = Recursive<(A a, B b), R>((k, lookup) => memorizableFn(k.a, k.b, (a, b) => lookup((a, b))));
        return (a, b) => fn((a, b));
    }

    public static Func<A, B, C, R> Recursive<A, B, C, R>(Func<A, B, C, Func<A, B, C, R>, R> memorizableFn)
    {
        var fn = Recursive<(A a, B b, C c), R>((k, lookup) => memorizableFn(k.a, k.b, k.c, (a, b, c) => lookup((a, b, c))));
        return (a, b, c) => fn((a, b, c));
    }

    public static Func<A, R> Recursive<A, R>(Func<A, Func<A, R>, R> memorizableFn)
    {
        Func<A, R> fn = null;
        var cache = new Dictionary<A, R>();
        fn = (a) =>
        {
            if (cache.TryGetValue(a, out var val))
                return val;
            return cache[a] = memorizableFn(a, fn);
        };
        return fn;
    }
}
