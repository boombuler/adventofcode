namespace AdventOfCode._2023;

using static Parser;

class Day22 : Solution
{
    record Brick(Point3D<int> Min, Point3D<int> Max)
    {        
        public bool Intersect(Brick other)
            => (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
               (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
               (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);

        public Brick Drop() => new ((Min.X, Min.Y, Min.Z-1), (Max.X, Max.Y, Max.Z-1));
    }

    private static readonly Func<string, Brick[]> BrickParser = (
        from s in IntPoint3D + "~"
        from e in IntPoint3D
        select new Brick(s,e)
    ).List('\n');

    private static (int singleBricks, DirectedGraph<Brick> graph) GetBrickSupports(string input)
    {
        var unsettled = BrickParser(input).OrderBy(b => b.Min.Z).ToList();
        var settled = new Dictionary<int, List<Brick>>();
        var graph = new DirectedGraph<Brick>();
        
        while (unsettled.Count > 0)
        {
            var next = new List<Brick>();
            foreach (var b in unsettled)
            {
                var dropped = b.Drop();
                var intersected = (settled.GetValueOrDefault(dropped.Min.Z)?.Where(dropped.Intersect) ?? Enumerable.Empty<Brick>());
                if (dropped.Min.Z == 0 || intersected.Any())
                {
                    settled.GetOrAdd(b.Max.Z, () => []).Add(b);
                    graph.AddRange(intersected, b);
                }
                else
                    next.Add(dropped);
            }
            unsettled = next;
        }
        // Some bricks might not touch any other brick:
        var unconnected = settled.SelectMany(n => n.Value).Where(n => !graph.Contains(n));
        return (unconnected.Count(), graph);
    }

    private static long CountChainReactionFallingBricks(string input)
    {
        var (_, graph) = GetBrickSupports(input);
        
        long ChainReaction(Brick b)
        {
            var falling = new HashSet<Brick>();
            var candidates = new HashSet<Brick>();
            bool LoosenBrick(Brick b)
            {
                falling.Add(b);
                candidates.Remove(b);
                return graph.Outgoing[b].Aggregate(false, (a, c) => a | candidates.Add(c));
            }

            bool anyLoosened = LoosenBrick(b);
            while(anyLoosened)
            {
                anyLoosened = candidates.Where(bs => graph.Incoming[bs].All(falling.Contains)).ToList()
                    .Aggregate(false, (a, r) => a | LoosenBrick(r));
            }
            return falling.Count - 1;
        }

        return graph.Vertices.AsParallel().Sum(ChainReaction);
    }

    protected override long? Part1()
    {
        static long Solve(string input)
        {
            var (s, graph) = GetBrickSupports(input);
            return s+graph.Vertices.Select(v => graph.Outgoing[v]).Count(v => v.All(s => graph.Incoming[s].Count() > 1));
        }
        Assert(Solve(Sample()), 5);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        Assert(CountChainReactionFallingBricks(Sample()), 7);
        return CountChainReactionFallingBricks(Input);
    }
}
