namespace AdventOfCode._2016;

class Day22 : Solution
{
    record Node(int X, int Y, int Used, int Avail, int Size, int UsePerc);
    private static readonly Func<string, Node?> NodeFactory
        = new Regex(@"/dev/grid/node-x(?<X>\d+)-y(?<Y>\d+)\s*(?<Size>\d+)T\s*(?<Used>\d+)T\s*(?<Avail>\d+)T\s*(?<UsePerc>\d+)%", RegexOptions.Compiled).ToFactory<Node>();

    private static int GetViablePairCount(string df)
    {
        var nodes = df.Lines().Skip(2).Select(NodeFactory).NonNull().ToList();
        return nodes.Where(na => na.Used > 0)
            .Sum(na => nodes.Where(nb => nb != na && nb.Avail >= na.Used).Count());
    }

    protected override long? Part1() => GetViablePairCount(Input);

    private int FindShortestPath(string df, bool debug = false)
    {
        var nodes = df.Lines().Skip(2).Select(NodeFactory).NonNull().ToList();
        var minSize = nodes.Min(n => n.Size);
        var maxX = nodes.Max(n => n.X);

        Node? wallStart = null;
        Node? empty = null;

        foreach (var grp in nodes.GroupBy(n => n.Y).OrderBy(g => g.Key))
        {
            var s = "";
            foreach (var node in grp.OrderBy(i => i.X))
            {
                if (node.Used > minSize)
                {
                    s += "#";
                    wallStart ??= node;
                }
                else if (node.Used == 0)
                {
                    s += "_";
                    empty = node;
                }
                else if (node.X == maxX && node.Y == 0)
                    s += "G";
                else
                    s += ".";
            }
            if (debug)
                Debug(s);
        }
        if (empty == null || wallStart == null)
            throw new InvalidOperationException("Could not find empty node or wall start.");

        return (empty.X - (wallStart.X - 1)) // Move left of wall
            + empty.Y                        // Move to top row
            + (maxX - wallStart.X + 1)       // Move to the right
            + (5 * (maxX - 1));              // Swap the data to left (takes 5 turns each time)

    }

    protected override long? Part2() => FindShortestPath(Input);
}
