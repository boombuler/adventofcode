namespace AdventOfCode._2025;

class Day11 : Solution
{
    private static DirectedGraph<string, Unit>  LoadGraph(string input)
    {
        var graph = new DirectedGraph<string, Unit>();
        foreach (var line in input.Lines())
        {
            (string src, (string dest, _)) = line.Split(':');
            graph.AddRange(src, dest.Split(' ', StringSplitOptions.RemoveEmptyEntries), Unit.Value);
        }
        return graph;
    }

    private static long PathsBetween(DirectedGraph<string, Unit> graph, string from, string to)
    {
        var paths = new Dictionary<string, long>() { [to] = 1 };
        var unresolved = graph.Vertices.ToHashSet();
        unresolved.Remove(to);
        while (unresolved.Count > 0)
        {
            foreach (var v in unresolved.ToArray())
            {
                if (graph.Outgoing[v].All(n => paths.ContainsKey(n)))
                {
                    paths[v] = graph.Outgoing[v].Sum(n => paths[n]);
                    unresolved.Remove(v);
                }
            }
        }
        return paths.GetValueOrDefault(from);
    }

    protected override long? Part1()
    {
        static long Solve(string input) => PathsBetween(LoadGraph(input), "you", "out");

        Assert(Solve(Sample("1")), 5);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input)
        {
            var g = LoadGraph(input);
            long CountPaths(params string[] way)
                => way.Pairwise((a, b) => PathsBetween(g, a, b)).Aggregate((a, b) => a * b);

            return CountPaths("svr", "dac", "fft", "out") +
                   CountPaths("svr", "fft", "dac", "out");
        }

        Assert(Solve(Sample("2")), 2);
        return Solve(Input);
    }
}
