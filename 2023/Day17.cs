namespace AdventOfCode._2023;

using Point = Point2D<int>;

class Day17 : Solution
{
    record Node(Point Pos, Point Dir, int Distance);

    class PathFinder(StringMap<int> Map, int MinDistance, int MaxDistance) 
        : AStar<Node>(new (Point.Origin, Point.Right, 0))
    {
        protected override long GuessDistance(Node from, Node to) 
            => from.Pos.ManhattanDistance(to.Pos);

        protected override long Distance(Node from, Node to)
            => Point.Range(from.Pos + to.Dir, to.Pos).Sum(p => Map[p]);

        protected override bool AreEqual(Node one, Node another) 
            => Equals(one.Pos, another.Pos);

        protected override IEnumerable<Node> NeighboursOf(Node node)
        {
            IEnumerable<Node> PossibleNeighbours()
            {
                if (node.Distance < MaxDistance)
                    yield return new (node.Pos + node.Dir, node.Dir, node.Distance + 1);

                var dir = node.Dir.RotateCW();
                yield return new (node.Pos + (MinDistance * dir),  dir, MinDistance);
                yield return new (node.Pos - (MinDistance * dir), -dir, MinDistance);
            }
            return PossibleNeighbours().Where(n => Map.Contains(n.Pos));
        }
    }

    private static long GetMinHeatLoss(string input, int minDistance, int maxDistance)
    { 
        var map = input.AsMap(c => c - '0');
        var pf = new PathFinder(map, minDistance, maxDistance);
        var dest = new Node(map.Size - (1, 1), Point.Origin, 0);
        return pf.ShortestPath(dest).Cost;
    }

    protected override long? Part1()
    {
        static long Solve(string input) => GetMinHeatLoss(input, 1, 3);

        Assert(Solve(Sample()), 102);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input) => GetMinHeatLoss(input, 4, 10);

        Assert(Solve(Sample()), 94);
        return Solve(Input);
    }
}
