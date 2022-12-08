namespace AdventOfCode._2022;

class Day03 : Solution
{
    private static int GetPrioritySum(IEnumerable<IEnumerable<IEnumerable<char>>> input)
        => input
            .Select(grp => grp.Aggregate(Enumerable.Intersect).Single())
            .Sum(item => char.IsLower(item) ? item - 'a' + 1 : item - 'A' + 27);

    protected override long? Part1()
    {
        static int GetSum(string s) 
            => GetPrioritySum(s.Lines().Select(l => l.Chunk(l.Length / 2)));

        Assert(GetSum(Sample()), 157);
        return GetSum(Input);
    }

    protected override long? Part2()
    {
        static int SumBadges(string s) 
            => GetPrioritySum(s.Lines().Chunk(3));

        Assert(SumBadges(Sample()), 70);
        return SumBadges(Input);
    }
}
