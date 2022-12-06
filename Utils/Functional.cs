namespace AdventOfCode.Utils;

static class Functional
{
    public static T Identity<T>(T e) => e;

    public static Func<T1, T3> Combine<T1, T2, T3>(this Func<T1, T2> first, Func<T2, T3> second)
        => (T1 v) => second(first(v));
}
