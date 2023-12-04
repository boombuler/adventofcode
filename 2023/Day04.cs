namespace AdventOfCode._2023;

using System;
using System.Collections.Generic;
using System.Linq;

using static Parser;

class Day04 : Solution
{
    private static readonly Func<string, int> CardParser =(
        from _ in Any.Until(":")
        from winning in Int.Token().Until("|")
        from own in Int.Token().Many()
        select winning.Intersect(own).Count()).MustParse;

    private static IEnumerable<int> WinningNumberCount(string input)
        => input.Lines().Select(CardParser);

    protected override long? Part1()
    { 
        static long Solve(string input)
            => WinningNumberCount(input).Where(n => n > 0).Sum(n => 1 << (n - 1));

        Assert(Solve(Sample()), 13);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input)
        {
            var wins = WinningNumberCount(input).ToList();
            var totals = wins.Select(n => (long)1).ToList();
            for (int i = 0; i < wins.Count; i++)
            {
                foreach (var idx in Enumerable.Range(i + 1, wins[i]).Where(n => n < wins.Count))
                    totals[idx] += totals[i];
            }
            return totals.Sum();
        }

        Assert(Solve(Sample()), 30);
        return Solve(Input);
    }
}
