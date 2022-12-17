namespace AdventOfCode._2022;

using System.Diagnostics;

class Day17 : Solution
{
    record Shape
    {
        public Point2D[] Points { get; init; }
        public int Height { get; }
        public Shape(params Point2D[] points)
        {
            Points = points;
            var (ymin, ymax) = points.MinMax(p => p.Y);
            Height = (int)((ymax - ymin) + 1);
        }
    }
    private readonly Shape[] Shapes = new Shape[]
    {
        new Shape((0,0), (1, 0), (2,0), (3, 0)), // -
        new Shape((1,0), (0, 1), (1, 1), (2, 1), (1, 2)), // +
        new Shape((2, 2), (2, 1), (0, 0), (1, 0), (2, 0)), // J
        new Shape((0,0), (0, 1), (0,2), (0,3)), // |
        new Shape((0,0), (0, 1), (1, 0), (1,1)), // box
    };

    class CacheKey
    {
        private readonly int fShapeIndex;
        private readonly int fJetIndex;
        private readonly ImmutableHashSet<Point2D> fPoints;
        public CacheKey(int ShapeIndex, int JetIndex, ImmutableHashSet<Point2D> Points)
        {
            fShapeIndex = ShapeIndex;
            fJetIndex = JetIndex;
            fPoints = Points;
        }
        public override bool Equals(object obj)
            => obj is CacheKey ck && 
                fShapeIndex == ck.fShapeIndex && 
                fJetIndex == ck.fJetIndex &&
                fPoints.SetEquals(ck.fPoints);

        public override int GetHashCode()
            => HashCode.Combine(fShapeIndex, fJetIndex, fPoints.Count);
    }

    private long Simulate(string input, long rockCount)
    {
        var jetstreams = input.Select(c => c == '>' ? new Point2D(1, 0) : (-1, 0)).ToArray();
        long height = 0;
        var blocked = Enumerable.Range(0, 7).Select(x => new Point2D(x, 0)).ToImmutableHashSet();
        Point2D blockedOffset = Point2D.Origin;
        
        bool CanMoveTo(Shape s, Point2D pos)
            => !s.Points.Select(p => p + pos - blockedOffset)
                .Any(p => p.X < 0 || p.X > 6 || blocked.Contains(p));

        Point2D CalcFinalPosition(Shape s, ref int jetIndex)
        {
            var pos = new Point2D(2, height + 4);
            while (true)
            {
                // Gas:
                jetIndex = (jetIndex + 1) % jetstreams.Length;
                var dir = jetstreams[jetIndex];
                var nextPos = pos + dir;
                if (CanMoveTo(s, nextPos))
                    pos = nextPos;
                // Fall Down:
                nextPos = pos - (0, 1);
                if (CanMoveTo(s, nextPos))
                    pos = nextPos;
                else
                    return pos;
            }
        }

        var cache = new Dictionary<CacheKey, (long Height, long RockNo)>();
        var j = -1;

        for (long r = 0; r < rockCount; r++)
        {
            var shapeIndex = (int)(r % Shapes.Length);
            var shape = Shapes[shapeIndex];
            var pos = CalcFinalPosition(shape, ref j);
            var top = pos.Y + shape.Height - 1;
            height = Math.Max(height, top);

            blocked = shape.Points.Aggregate(blocked, (b, p) => b.Add(p + pos - blockedOffset));

            const int PatternLines = 50; // Lines of blocked cells to account for cache and collision checking
            var max = blocked.Max(b => b.Y);
            if (max > PatternLines)
            {
                var delta = new Point2D(0, max - PatternLines);
                blockedOffset += delta;
                blocked = blocked.Select(b => b - delta).Where(p => p.Y >= 0).ToImmutableHashSet();
            }

            var key = new CacheKey(shapeIndex, j, blocked);
            if (!cache.TryAdd(key, (height, r)))
            {
                var (h, rn) = cache[key];
                var cycleLength = (r - rn);
                var skipCycles = (rockCount - r) / cycleLength;
                var skipedHeight = (height - h) * skipCycles;
                height += skipedHeight;
                blockedOffset += (0, skipedHeight);
                r += cycleLength * skipCycles;
            }
        }
        return height;
    }

    protected override long? Part1()
    {
        Assert(Simulate(Sample(), 2), 4);
        Assert(Simulate(Sample(), 2022), 3068);
        return Simulate(Input, 2022);
    }

    protected override long? Part2()
    {
        Assert(Simulate(Sample(), 1000000000000L), 1514285714288);
        return Simulate(Input, 1000000000000L);
    }
}
