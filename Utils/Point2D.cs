namespace AdventOfCode.Utils;

public record Point2D(long X, long Y) : IComparable<Point2D>
{
    public static readonly Point2D Origin = (0, 0);

    public override string ToString() => $"{X},{Y}";

    public static implicit operator (long, long)(Point2D pt) => (pt.X, pt.Y);
    public static implicit operator Point2D((long, long) t) => new(t.Item1, t.Item2);

    public static Point2D operator -(Point2D a, Point2D b)
        => new(a.X - b.X, a.Y - b.Y);
    public static Point2D operator +(Point2D a, Point2D b)
        => new(a.X + b.X, a.Y + b.Y);
    public static Point2D operator *(Point2D a, long b)
        => new(a.X * b, a.Y * b);
    public static Point2D operator *(long a, Point2D b)
        => b * a;

    public IEnumerable<Point2D> Neighbours(bool withDiagonal = false)
    {
        yield return this + (1, 0);
        yield return this + (0, 1);
        yield return this - (1, 0);
        yield return this - (0, 1);
        if (withDiagonal)
        {
            yield return this + (+1, +1);
            yield return this + (-1, +1);
            yield return this + (+1, -1);
            yield return this + (-1, -1);
        }
    }

    public long ManhattanDistance(Point2D other)
        => Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
    public double Length => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));

    public static IEnumerable<Point2D> Range(Point2D min, Point2D max)
    {
        (var minX, var maxX) = min.X < max.X ? (min.X, max.X) : (max.X, min.X);
        (var minY, var maxY) = min.Y < max.Y ? (min.Y, max.Y) : (max.Y, min.Y);
        for (long x = minX; x <= maxX; x++)
            for (long y = minY; y <= maxY; y++)
                yield return new Point2D(x, y);
    }

    public int CompareTo(Point2D other)
    {
        var dy = Y - other.Y;
        if (dy == 0)
            return Math.Sign(X - other.X);
        return Math.Sign(dy);
    }

    public Point3D WithZ(long z) => new(X, Y, z);

    public static Func<Point2D, bool> InBounds(Point2D min, Point2D max)
    {
        var (minX, maxX) = min.X < max.X ? (min.X, max.X) : (max.X, min.X);
        var (minY, maxY) = min.Y < max.Y ? (min.Y, max.Y) : (max.Y, min.Y);
        return (pt) => pt.X >= minX && pt.X <= maxX && pt.Y >= minY && pt.Y <= maxY;
    }
}
