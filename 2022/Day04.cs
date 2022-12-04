namespace AdventOfCode._2022;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day04 : Solution
{
    record Range(int Min, int Max) 
    { 
        public static readonly Func<string, Range> Factory 
            = new Regex(@"(?<Min>\d+)-(?<Max>\d+)").ToFactory<Range>();

        public bool Contains(int n) => n >= Min && n <= Max;
        public static bool Contains(Range a, Range b) => a.Contains(b.Min) && a.Contains(b.Max);
        public static bool Overlaps(Range a, Range b) => a.Contains(b.Min) || a.Contains(b.Max);
    }

    private int CountRanges(string input, Func<Range, Range,bool> compare)
        => input.Lines()
            .Select(l => l.Split(',').Select(Range.Factory).MinMaxBy(n => n.Min))
            .Count(n => compare(n.min,n.max));

    protected override long? Part1()
    {
        Assert(2, CountRanges(Sample(), Range.Contains));
        return CountRanges(Input, Range.Contains);
    }

    protected override long? Part2()
    {
        Assert(4, CountRanges(Sample(), Range.Overlaps));
        return CountRanges(Input, Range.Overlaps);
    }
}
