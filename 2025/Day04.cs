namespace AdventOfCode._2025;


class Day04 : Solution
{
    private static IEnumerable<int> CountRemovedPaperRolls(string input)
    {
        var rolls = StringMap.Create(input).Where(x => x.Value == '@').Select(x => x.Index).ToImmutableHashSet();
        return rolls
            .Unfold(map => map.Except(map.Where(x => x.Neighbours(true).Count(map.Contains) < 4)))
            .Prepend(rolls)
            .Pairwise((a, b) => a.Count - b.Count);
    }

    protected override long? Part1()
    {
        static long Solve(string input) => CountRemovedPaperRolls(input).First();

        Assert(Solve(Sample()), 13);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input) => CountRemovedPaperRolls(input).TakeWhile(x => x > 0).Sum();

        Assert(Solve(Sample()), 43);
        return Solve(Input);
    }
}
