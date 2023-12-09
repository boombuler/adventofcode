namespace AdventOfCode._2018;

class Day22 : Solution
{
    enum RegionType
    {
        Rocky = 0,
        Wet = 1,
        Narrow = 2
    }

    [Flags]
    enum Tools
    {
        Climbing = 1,
        Torch = 2,
        None = 4
    }

    private static readonly Tools[] ValidTools =
    [
        Tools.Torch | Tools.Climbing, // Rocky
        Tools.None | Tools.Climbing, // Wet
        Tools.None | Tools.Torch, // Narrow
    ];

    class CaveRouting(Cave cave, Point2D src, Tools startingEquip) : AStar<(Point2D Pt, Tools Tool)>((src, startingEquip))
    {
        private const long TOOL_CHANGE_COST = 7;
        private readonly Cave fCave = cave;

        protected override long Distance((Point2D Pt, Tools Tool) one, (Point2D Pt, Tools Tool) another)
            => one.Pt.ManhattanDistance(another.Pt) + (one.Tool == another.Tool ? 0 : TOOL_CHANGE_COST);

        protected override IEnumerable<(Point2D Pt, Tools Tool)> NeighboursOf((Point2D Pt, Tools Tool) node)
        {
            var fromType = fCave.GetRegionType(node.Pt);

            yield return (node.Pt, node.Tool ^ ValidTools[(int)fromType]);

            foreach (var cell in node.Pt.Neighbours(false))
            {
                if (cell.X < 0 || cell.Y < 0)
                    continue;

                if (ValidTools[(int)fCave.GetRegionType(cell)].HasFlag(node.Tool))
                    yield return (cell, node.Tool);
            }
        }
    }

    class Cave(long Depth, long X, long Y)
    {
        private readonly long fDepth = Depth;
        private readonly Point2D fTarget = (X, Y);
        private readonly Dictionary<Point2D, int> fErosionLevels = [];
        public static readonly Func<string, Cave> Parse = new Regex(@"depth: (?<Depth>\d+)\ntarget: (?<X>\d+),(?<Y>\d+)", RegexOptions.Multiline).ToFactory<Cave>();

        private long GetGeologicIndex(Point2D pt)
        {
            if (pt == Point2D.Origin || pt == fTarget)
                return 0;
            if (pt.Y == 0)
                return pt.X * 16807;
            if (pt.X == 0)
                return pt.Y * 48271;
            return GetErosionLevel(pt - (0, 1)) * GetErosionLevel(pt - (1, 0));
        }

        private int GetErosionLevel(Point2D pt)
        {
            if (!fErosionLevels.TryGetValue(pt, out var result))
                fErosionLevels[pt] = result = (int)((GetGeologicIndex(pt) + fDepth) % 20183);
            return result;
        }

        public RegionType GetRegionType(Point2D pt)
            => (RegionType)(GetErosionLevel(pt) % 3);

        public long GetRiskLevel()
            => Point2D.Range(Point2D.Origin, fTarget).Select(GetRegionType).Cast<int>().Sum();

        public long GetRescueTime()
            => new CaveRouting(this, Point2D.Origin, Tools.Torch).ShortestPath((fTarget, Tools.Torch)).Cost;
    }

    protected override long? Part1()
    {
        var c = Cave.Parse(Sample());
        Assert(c.GetRegionType((0, 0)), RegionType.Rocky);
        Assert(c.GetRegionType((1, 0)), RegionType.Wet);
        Assert(c.GetRegionType((0, 1)), RegionType.Rocky);
        Assert(c.GetRegionType((1, 1)), RegionType.Narrow);
        Assert(c.GetRegionType((10, 10)), RegionType.Rocky);
        Assert(c.GetRiskLevel(), 114);
        return Cave.Parse(Input).GetRiskLevel();
    }

    protected override long? Part2()
    {
        Assert(Cave.Parse(Sample()).GetRescueTime(), 45);
        return Cave.Parse(Input).GetRescueTime();
    }
}
