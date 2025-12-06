namespace AdventOfCode._2022;

class Day04 : Solution
{
    record Range(int Min, int Max) 
    { 
        public static readonly Func<string, Range?> Factory 
            = new Regex(@"(?<Min>\d+)-(?<Max>\d+)").ToFactory<Range>();

        public bool Contains(int n) => n >= Min && n <= Max;
        public static bool Contains(Range a, Range b) => a.Contains(b.Min) && a.Contains(b.Max);
        public static bool Overlaps(Range a, Range b) => a.Contains(b.Min) || a.Contains(b.Max);
    }

    private static int CountRanges(string input, Func<Range, Range,bool> compare)
        => input.Lines()
            .Select(l => l.Split(',').Select(Range.Factory).NonNull().MinMaxBy(n => n.Min))
            .Count(n => compare(n.min,n.max));

    protected override long? Part1()
    {
        Assert(CountRanges(Sample(), Range.Contains), 2);
        return CountRanges(Input, Range.Contains);
    }

    protected override long? Part2()
    {
        Assert(CountRanges(Sample(), Range.Overlaps), 4);
        return CountRanges(Input, Range.Overlaps);
    }
}
