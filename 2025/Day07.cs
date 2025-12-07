namespace AdventOfCode._2025;

class Day07 : Solution
{
    record Tachyon(int X, long Timelines, int SplitCount);
    private static IEnumerable<Tachyon> RunSimulation(string input)
    {
        var map = input.AsMap();
        var start = map.Find('S') ?? throw new InvalidInputException("No start found");

        return Enumerable.Range(start.Y, map.Size.Y - start.Y)
            .Aggregate<int, Tachyon[]>([new (start.X, 1, 0)], (tachyons, y) => [.. tachyons
                .SelectMany(t => map[(t.X, y)] switch
                {
                    '^' => [t with { X = t.X - 1, SplitCount = 1 }, t with { X = t.X + 1 }],
                    _ => new Tachyon[] { t }
                })
                .GroupBy(t => t.X)
                .Select(grp => new Tachyon(grp.Key, grp.Sum(g => g.Timelines), grp.Sum(g => g.SplitCount)))]);
    }

    protected override long? Part1()
    {
        static long Solve(string input) => RunSimulation(input).Sum(t => t.SplitCount);

        Assert(Solve(Sample()), 21);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input) => RunSimulation(input).Sum(t => t.Timelines);

        Assert(Solve(Sample()), 40);
        return Solve(Input);
    }
}
