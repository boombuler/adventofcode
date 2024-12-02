namespace AdventOfCode._2023;

using Point2D = Point2D<long>;

class Day11 : Solution
{
    private static long Solve(string input, int factor = 2)
    {
        var galaxies = input.Cells(filter: v => v == '#').Keys;
        List<int> GetInflation(Func<Point2D<int>, int> selector)
            => Enumerable.Range(0, galaxies.Max(x => selector(x))).Except(galaxies.Select(x => selector(x))).ToList();
        var rows = GetInflation(p => (int)p.Y);
        var cols = GetInflation(p => (int)p.X);

        return galaxies.Select(g => (g.X, g.Y) - (factor - 1) *
            new Point2D((long)cols.BinarySearch((int)g.X), (long)rows.BinarySearch((int)g.Y))
        ).CombinationPairs().Select((ab) => ab.A.ManhattanDistance(ab.B)).Sum();
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample()), 374);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        Assert(Solve(Sample(), 10), 1030);
        Assert(Solve(Sample(), 100), 8410);
        return Solve(Input, 1_000_000);
    }
}
