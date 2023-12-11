namespace AdventOfCode._2023;

class Day11 : Solution
{
    private static long Solve(string input, long factor = 2)
    {
        var galaxies = input.Cells(filter: v => v == '#').Keys;
        var inflateRows = Enumerable.Range(0, galaxies.Max(g => (int)g.Y)).Except(galaxies.Select(g => (int)g.Y));
        var inflateCols = Enumerable.Range(0, galaxies.Max(g => (int)g.X)).Except(galaxies.Select(g => (int)g.X));

        return galaxies.Select(g => g + (factor-1) * 
            new Point2D(inflateCols.Count(c => c < g.X), inflateRows.Count(r => r < g.Y))
        ).Pairs().Select((ab) => ab.A.ManhattanDistance(ab.B)).Sum();
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
        return Solve(Input, 1000000);
    }
}
