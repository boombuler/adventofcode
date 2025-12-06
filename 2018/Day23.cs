namespace AdventOfCode._2018;

using Point = Point3D<long>;

class Day23 : Solution
{
    class BoundingBox
    {
        private readonly Point fSize;
        public Point Min { get; }
        public Point Max { get; }
        public long DistanceToOrigin { get; }
        public bool IsPoint => fSize.X == 1 && fSize.Y == 1 && fSize.Z == 1;

        public BoundingBox(Point min, Point size)
        {
            Min = min;
            Max = min + size - (1, 1, 1);
            fSize = size;
            DistanceToOrigin = Math.Min(
                Max.ManhattanDistance(Point.Origin),
                Min.ManhattanDistance(Point.Origin)
            );
        }

        public IEnumerable<BoundingBox> Divide()
        {
            BoundingBox MkBox(int i)
            {
                bool IsSet(int bit) => ((i >> bit) & 1) != 0;
                var pos = fSize.MapComponents((v, b) => IsSet(b) ? v / 2 : 0);
                var size = fSize.MapComponents((v, b) => v / (IsSet(b) ? 1 : 2));
                return new BoundingBox(Min + pos, size - pos);
            }
            return Enumerable.Range(0, 8).Select(MkBox).Where(box => box.fSize.X > 0 && box.fSize.Y > 0 && box.fSize.Z > 0);
        }
    }

    class NanoBot
    {
        public static readonly Func<string, NanoBot?> Parse = new Regex(@"pos=\<(?<x>-?\d+),(?<y>-?\d+),(?<z>-?\d+)\>, r=(?<r>\d+)").ToFactory<NanoBot>();
        public Point Location { get; }
        public long Radius { get; }
        public NanoBot(long x, long y, long z, long r)
            => (Location, Radius) = ((x, y, z), r);

        public bool Reaches(Point pt) => Location.ManhattanDistance(pt) <= Radius;
        public bool Intersects(BoundingBox box)
        {
            var dx = Math.Max(0, Math.Max(box.Min.X - Location.X, Location.X - box.Max.X));
            var dy = Math.Max(0, Math.Max(box.Min.Y - Location.Y, Location.Y - box.Max.Y));
            var dz = Math.Max(0, Math.Max(box.Min.Z - Location.Z, Location.Z - box.Max.Z));

            return dx + dy + dz <= Radius;
        }
    }

    public static int CountInRangeOfStrongestSignal(string nanoBots)
    {
        var bots = nanoBots.Lines().Select(NanoBot.Parse).NonNull().ToList();
        var (_, maxBot) = bots.MinMaxBy(b => b.Radius);
        return bots.Select(b => b.Location).Count(maxBot.Reaches);
    }

    public static long DistanceToBestSpot(string nanoBots)
    {
        var bots = nanoBots.Lines().Select(NanoBot.Parse).NonNull().ToArray();

        var (minX, maxX) = bots.MinMax(b => b.Location.X);
        var (minY, maxY) = bots.MinMax(b => b.Location.Y);
        var (minZ, maxZ) = bots.MinMax(b => b.Location.Z);
        var min = new Point(minX, minY, minZ);
        var max = new Point(maxX, maxY, maxZ);

        var q = new PriorityQueue<(BoundingBox BBox, NanoBot[] Bots), (int Count, long Distance)>(
            ComparerBuilder<(int Count, long Distance)>
                .CompareByDesc(x => x.Count)
                .ThenBy(x => x.Distance)
        );
        void Push(BoundingBox bb, NanoBot[] bots)
            => q.Enqueue((bb, bots), (bots.Length, bb.DistanceToOrigin));

        Push(new BoundingBox(min, max - min + (1, 1, 1)), bots);

        while (q.TryDequeue(out var cur, out _))
        {
            if (cur.BBox.IsPoint)
                return cur.BBox.DistanceToOrigin;

            foreach (var box in cur.BBox.Divide())
                Push(box, [.. cur.Bots.Where(b => b.Intersects(box))]);
        }

        return -1;
    }

    protected override long? Part1()
    {
        Assert(CountInRangeOfStrongestSignal(Sample(nameof(Part1))), 7);
        return CountInRangeOfStrongestSignal(Input);
    }

    protected override long? Part2()
    {
        Assert(DistanceToBestSpot(Sample(nameof(Part2))), 36);
        return DistanceToBestSpot(Input);
    }
}
