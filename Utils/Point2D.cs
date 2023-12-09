namespace AdventOfCode.Utils;

public record Point2D(long X, long Y) : IComparable<Point2D>
{
    public static readonly Point2D Origin = (0, 0);
    public static readonly Point2D Up = (0, -1);
    public static readonly Point2D Down = (0, 1);
    public static readonly Point2D Left = (-1, 0);
    public static readonly Point2D Right = (1, 0);

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
    public static Point2D operator /(Point2D a, long b)
        => new(a.X/b, a.Y / b);
    public static Point2D operator %(Point2D a, long b)
        => new(a.X % b, a.Y % b);
    public static Point2D operator %(Point2D a, Point2D b)
        => new(a.X % b.X, a.Y % b.Y);

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

    public static Point2D Parse(string s)
    {
        var parts = s.Split(',');
        return new Point2D(long.Parse(parts[0]), long.Parse(parts[1]));
    }

    /// <summary>
    /// Maps '<', '>', '^' and 'v' into directions. Any other char returns (0,0)
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static Point2D DirectionFromArrow(char c)
        => c switch
        {
            '^' => Up,
            'v' => Down,
            '<' => Left,
            '>' => Right,
            _ => (0, 0)
        };
}
