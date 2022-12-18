namespace AdventOfCode.Utils;

public record Point3D(long X, long Y, long Z)
{
    public static readonly Point3D Origin = (0, 0, 0);

    public override string ToString() => $"{X}|{Y}|{Z}";

    public static implicit operator (long, long, long)(Point3D pt) => (pt.X, pt.Y, pt.Z);
    public static implicit operator Point3D((long, long, long) t) => new(t.Item1, t.Item2, t.Item3);

    public static Point3D operator -(Point3D a, Point3D b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Point3D operator +(Point3D a, Point3D b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Point3D operator *(Point3D a, long b)
        => new(a.X * b, a.Y * b, a.Z * b);
    public static Point3D operator *(long a, Point3D b)
        => b * a;

    public long ManhattanDistance(Point3D other)
        => Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);

    public Point3D MapComponents(Func<long, int, long> selector)
        => new(selector(X, 0), selector(Y, 1), selector(Z, 2));

    public void Deconstruct(out Point2D pt, out long z)
        => (pt, z) = ((X, Y), Z);

    public static IEnumerable<Point3D> Range(Point3D min, Point3D max)
    {
        (var minX, var maxX) = min.X < max.X ? (min.X, max.X) : (max.X, min.X);
        (var minY, var maxY) = min.Y < max.Y ? (min.Y, max.Y) : (max.Y, min.Y);
        (var minZ, var maxZ) = min.Z < max.Z ? (min.Z, max.Z) : (max.Z, min.Z);
        for (long x = minX; x <= maxX; x++)
            for (long y = minY; y <= maxY; y++)
                for (long z = minZ; z <= maxZ; z++)
                    yield return new Point3D(x, y, z);
    }

    public static Func<Point3D, bool> InBounds(Point3D min, Point3D max)
    {
        var (minX, maxX) = min.X < max.X ? (min.X, max.X) : (max.X, min.X);
        var (minY, maxY) = min.Y < max.Y ? (min.Y, max.Y) : (max.Y, min.Y);
        var (minZ, maxZ) = min.Z < max.Z ? (min.Z, max.Z) : (max.Z, min.Z);
        return (pt) => 
            pt.X >= minX && pt.X <= maxX && 
            pt.Y >= minY && pt.Y <= maxY &&
            pt.Z >= minZ && pt.Z <= maxZ;
    }

    public IEnumerable<Point3D> Neighbours()
    {
        yield return this + (0, 0, 1);
        yield return this - (0, 0, 1);
        yield return this + (0, 1, 0);
        yield return this - (0, 1, 0);
        yield return this + (1, 0, 0);
        yield return this - (1, 0, 0);
    }

    public static Point3D Parse(string s)
    {
        var parts = s.Split(',');
        return new Point3D(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]));
    }

    public static (Point3D Min, Point3D Max) Bounds(IEnumerable<Point3D> points)
    {
        var (minX, maxX) = points.MinMax(p => p.X);
        var (minY, maxY) = points.MinMax(p => p.Y);
        var (minZ, maxZ) = points.MinMax(p => p.Z);
        return (new Point3D(minX, minY, minZ), new Point3D(maxX, maxY, maxZ));
    }
}
