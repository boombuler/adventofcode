namespace AdventOfCode._2021;

class Day12 : Solution
{
    private const string START = "start";
    private const string END = "end";

    private static long CountUniquePaths(string map, int smallCaveRevisitCount = 0)
    {
        var edges = map.Lines().Select(l => l.Split('-'))
            .SelectMany(l => new[] { (A: l[0], B: l[1]), (A: l[1], B: l[0]) })
            .Where(n => n.B != START && n.A != END) // don't revisit start cave
            .ToLookup(n => n.A, n => n.B);

        var pathCount = 0;
        var open = new Queue<(string Cave, int RevisitCount, ImmutableArray<string> SmallCaves)>();
        open.Enqueue((START, 0, ImmutableArray<string>.Empty));
        while (open.TryDequeue(out var current))
        {
            foreach (var segment in edges[current.Cave])
            {
                if (segment == END)
                {
                    pathCount++;
                    continue;
                }

                var (_, count, smallCaves) = current;
                if (char.IsLower(segment[0]))
                {
                    if (smallCaves.Contains(segment))
                    {
                        if (++count > smallCaveRevisitCount)
                            continue;
                    }
                    else
                        smallCaves = smallCaves.Add(segment);
                }
                open.Enqueue((segment, count, smallCaves));
            }
        }
        return pathCount;
    }

    protected override long? Part1()
    {
        Assert(CountUniquePaths(Sample("1")), 10);
        Assert(CountUniquePaths(Sample("2")), 19);
        Assert(CountUniquePaths(Sample("3")), 226);
        return CountUniquePaths(Input);
    }

    protected override long? Part2()
    {
        Assert(CountUniquePaths(Sample("1"), 1), 36);
        Assert(CountUniquePaths(Sample("2"), 1), 103);
        Assert(CountUniquePaths(Sample("3"), 1), 3509);
        return CountUniquePaths(Input, 1);
    }
}
