namespace AdventOfCode._2021;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AdventOfCode.Utils;

class Day14 : Solution
{
    private ImmutableDictionary<char, long> Add(ImmutableDictionary<char, long> a, ImmutableDictionary<char, long> b)
    {
        if (a == null || b == null)
            return a ?? b;
        foreach (var (key, count) in b)
            a = a.SetItem(key, a.GetValueOrDefault(key) + count);
        return a;
    }

    private long Solve(string input, int rounds)
    {
        var template = input.Lines().First();
        var rules = input.Lines().Skip(2).Select(n => n.Split(" -> ")).ToDictionary(p => (A: p[0][0], B: p[0][1]), p => p[1][0]);

        static ImmutableDictionary<char, long> CharToDict(char c) => ImmutableDictionary<char, long>.Empty.SetItem(c, 1);

        var production = new Dictionary<(char, char), ImmutableDictionary<char, long>>();
        for (int i = 0; i < rounds; i++)
        {
            production = rules.ToDictionary(kvp => kvp.Key, kvp => new[]{
                    production.GetValueOrDefault((kvp.Key.A, kvp.Value)),
                    production.GetValueOrDefault((kvp.Value, kvp.Key.B)),
                    CharToDict(kvp.Value)
                }.Aggregate(Add));
        }

        var (min, max) = template.SlidingWindow(2).Select(wnd => (wnd[0], wnd[1])).Select(production.GetValueOrDefault)
            .Concat(template.Select(CharToDict))
            .Aggregate(Add).Values.MinMax();

        return max - min;
    }
    protected override long? Part1()
    {
        Assert(Solve(Sample(), 10), 1588);
        return Solve(Input, 10);
    }

    protected override long? Part2()
    {
        Assert(Solve(Sample(), 40), 2188189693529);
        return Solve(Input, 40);
    }
}
