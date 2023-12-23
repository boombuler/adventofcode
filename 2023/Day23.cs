namespace AdventOfCode._2023;

using Point = Point2D<int>;
using Graph = DirectedGraph<Point2D<int>, int>;

class Day23 : Solution
{
    private static bool IsSlopeBlocked(char tile, Point dir)
        => tile switch
        {
            '>' when dir == Point.Left => true,
            '<' when dir == Point.Right => true,
            '^' when dir == Point.Down => true,
            'v' when dir == Point.Up => true,
            _ => false
        };

    private static Graph CreateGraphFromInput(string input, bool ignoreSlopes)
    {
        var map = input.AsMap();
        var start = Enumerable.Range(0, map.Width).Select(x => new Point(x, 0)).First(p => map[p] == '.');
        var end = Enumerable.Range(0, map.Width).Select(x => new Point(x, map.Height - 1)).First(p => map[p] == '.');

        var graph = new DirectedGraph<Point, int>();

        var seen = new HashSet<(Point pos, Point srcVert)>();        
        var open = new Queue<(Point pos, Point lastVertex, int steps)>();

        open.Enqueue((start, start, 0));
        while (open.TryDequeue(out var cur))
        {
            var (pos, vert, steps) = cur;
            if (pos == end)
            {
                graph.Add(vert, pos, steps);
                continue;
            }

            var next = (
                from n in pos.Neighbours()
                let tile = map.GetValueOrDefault(n, '#')
                where tile != '#'
                let d = n - pos
                select new
                {
                    Pos = n,
                    Blocked = !ignoreSlopes && IsSlopeBlocked(tile, d)
                }
            ).ToList();

            if (next.Count > 2)
            {
                graph.Add(vert, pos, steps);
                vert = pos;
                steps = 0;
            }
            
            foreach(var n in next)
            {
                if (!n.Blocked && seen.Add((n.Pos, vert)))
                    open.Enqueue((n.Pos, vert, steps+1));
            }
        }
        return graph;
    }

    private static HashSet<Point> GetReachableNodes(Graph graph, Point pos, ImmutableHashSet<Point> visited)
    {
        HashSet<Point> result = [];
        var open = new Stack<Point>([pos]);

        while (open.TryPop(out var p))
        {
            foreach(var o in graph.Outgoing[p].Where(o => !visited.Contains(o) && result.Add(o)))
                open.Push(o);
        }
        return result;
    }

    class State(Point pos, HashSet<Point> open)
    {
        static readonly IEqualityComparer<HashSet<Point>> HashSetComp = HashSet<Point>.CreateSetComparer();
        public Point Pos { get; } = pos;
        public HashSet<Point> Open { get; } = open;
        public override bool Equals(object obj)
            => obj is State s && Pos == s.Pos && HashSetComp.Equals(Open, s.Open);
        public override int GetHashCode()
            => HashCode.Combine(Pos, HashSetComp.GetHashCode(Open));
    }

    private static long GetLongestPath(string input, bool ignoreSlopes)
    {
        var graph = CreateGraphFromInput(input, ignoreSlopes);
        var start = graph.Sources.Single();
        var end = graph.Sinks.Single();

        var open = new Queue<(Point pos, ImmutableHashSet<Point> path, int cost)>();
        open.Enqueue((start, ImmutableHashSet<Point>.Empty, 0));
        int maxCost = 0;

        Dictionary<State, int> seen = [];

        while (open.TryDequeue(out var cur))
        {
            if (cur.pos == end)
                maxCost = Math.Max(cur.cost, maxCost);

            var openNodes = GetReachableNodes(graph, cur.pos, cur.path);
            var state = new State(cur.pos, openNodes);
            if (seen.GetValueOrDefault(state, -1) >= cur.cost)
                continue;
            seen[state] = cur.cost;

            foreach (var n in graph.Outgoing[cur.pos].Where(n => !cur.path.Contains(n)))
                open.Enqueue((n, cur.path.Add(n), graph[cur.pos, n] + cur.cost));
        }

        return maxCost;
    }

    protected override long? Part1()
    {
        Assert(GetLongestPath(Sample(), false), 94);
        return GetLongestPath(Input, false);
    }

    protected override long? Part2()
    {
        Assert(GetLongestPath(Sample(), true), 154);
        return GetLongestPath(Input, true);
    }
}
