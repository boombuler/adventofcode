namespace AdventOfCode._2016;

class Day24 : Solution
{
    class Node(char c, Point2D<int> l)
    {
        public int? Number { get; } = c is not '.' ? c - '0' : null;
        public Point2D<int> Location { get; } = l;
        public List<Node> Neigbours { get; } = [];
    }

    class NodeAStar(Node src) : AStar<Node>(src)
    {
        protected override long Distance(Node one, Node another) => one.Location.ManhattanDistance(another.Location);

        protected override IEnumerable<Node> NeighboursOf(Node node) => node.Neigbours;
    }

    private static Dictionary<int, Node> ReadGrid(string map)
    {
        var cells = map.Cells(filter: v => v != '#').ToDictionary(kvp => kvp.Key, kvp => new Node(kvp.Value, kvp.Key));
        var result = new Dictionary<int, Node>();
        foreach (var kvp in cells)
        {
            foreach (var n in kvp.Key.Neighbours())
            {
                if (cells.TryGetValue(n, out var other))
                    kvp.Value.Neigbours.Add(other);
            }
            if (kvp.Value.Number.HasValue)
                result[kvp.Value.Number.Value] = kvp.Value;
        }
        return result;
    }

    public static long GetShortestPath(string map, bool returnToZero = false)
    {
        var grid = ReadGrid(map);
        var distances = new Dictionary<(int, int), long>();
        foreach (var src in grid.Keys)
        {
            var astar = new NodeAStar(grid[src]);
            foreach (var dest in grid.Keys)
            {
                if (dest == src)
                    continue;

                distances[(src, dest)] = astar.ShortestPath(grid[dest]).Cost;
            }
        }

        var minCost = long.MaxValue;
        foreach (var travlePath in grid.Keys.Where(k => k != 0).Permuatate())
        {
            int last = 0;
            long cost = 0;
            foreach (var n in travlePath)
            {
                cost += distances[(last, n)];
                last = n;
            }
            if (returnToZero)
                cost += distances[(last, 0)];

            minCost = Math.Min(minCost, cost);
        }
        return minCost;
    }

    protected override long? Part1()
    {
        Assert(GetShortestPath(Sample()), 14);
        return GetShortestPath(Input);
    }

    protected override long? Part2() => GetShortestPath(Input, true);

}
