namespace AdventOfCode._2023;

using Point = Point2D<int>;
using Graph = DirectedGraph<long, int>;

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

        var graph = new Graph();

        var seen = new HashSet<(Point pos, Point srcVert)>();        
        var open = new Queue<(Point pos, Point srcVert, int steps)>();
        var vertices = new Dictionary<Point, long>();

        long Bit(Point f) => vertices.GetOrAdd(f, () => 1L << vertices.Count);
        
        open.Enqueue((start, start, 0));
        while (open.TryDequeue(out var cur))
        {
            var (pos, vert, steps) = cur;
            
            var next = (
                from Pos in pos.Neighbours()
                let tile = map.GetValueOrDefault(Pos, '#')
                where pos != end && tile != '#'
                let d = Pos - pos
                select new {Pos,  Blocked = !ignoreSlopes && IsSlopeBlocked(tile, d) }
            ).ToList();

            if (next.Count > 2 || pos == end)
            {
                graph.Add(Bit(cur.srcVert), Bit(cur.pos), cur.steps);
                (vert, steps) = (pos, 0);
            }
            
            foreach(var n in next)
            {
                if (!n.Blocked && seen.Add((n.Pos, vert)))
                    open.Enqueue((n.Pos, vert, steps+1));
            }
        }
        return graph;
    }

    private long GetLongestPath(string input, bool ignoreSlopes)
    {
        var graph = CreateGraphFromInput(input, ignoreSlopes);
        var start = graph.Sources.Single();
        var end = graph.Sinks.Single();

        long GetReachableNodes(long pos, long visited)
        {
            long result = 0;
            var open = new Stack<long>([pos]);

            while (open.TryPop(out var p))
            {
                foreach (var o in graph.Outgoing[p])
                {
                    if ((visited & o) == 0 && (result & o) == 0)
                    {
                        result |= o;
                        open.Push(o);
                    }
                }   
            }
            return result;
        }

        var open = new Stack<(long pos, long path, int cost)>([(start, 0, 0)]);
        int maxCost = 0;

        var seen = new Dictionary<(long, long), int>();

        while (open.TryPop(out var cur))
        {
            if (cur.pos == end)
                maxCost = Math.Max(cur.cost, maxCost);

            var reachable = GetReachableNodes(cur.pos, cur.path);
            if (seen.GetValueOrDefault((cur.pos, reachable), -1) >= cur.cost)
                continue;
            seen[(cur.pos, reachable)] = cur.cost;

            foreach (var n in graph.Outgoing[cur.pos].Where(n => (n & cur.path) == 0))
                open.Push((n, cur.path | n, graph[cur.pos, n] + cur.cost));
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
