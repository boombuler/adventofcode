namespace AdventOfCode._2024;

using static Parser;

class Day01 : Solution
{
    private long CompareLists<T>(string input, Func<IEnumerable<int>, T> prepareList, 
        Func<T, T, IEnumerable<int>> zip) => (
            from left in Int.Token()
            from right in Int.Token()
            select (Left: left, Right: right)
        ).List('\n').MustParse(input)
        .Aggregate(
            (Left: Enumerable.Empty<int>(), Right: Enumerable.Empty<int>()),
            (acc, e) => (acc.Left.Append(e.Left), acc.Right.Append(e.Right)),
            (acc) => zip(prepareList(acc.Left), prepareList(acc.Right)))
        .Sum();

    protected override long? Part1()
    {
        long Solve(string input)
            => CompareLists(input, 
                lst => lst.Order(), 
                (left, right) => left.Zip(right, (int l, int r) => Math.Abs(l - r)));

        Assert(Solve(Sample()), 11);
        return Solve(Input);
    }

    protected override long? Part2() 
    {
        long Solve(string input)
            => CompareLists(input, 
                lst => lst.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()),
                (left, right) => left.Select(l => l.Key * l.Value * right.GetValueOrDefault(l.Key, 0)));

        Assert(Solve(Sample()), 31);
        return Solve(Input);
    }
}
