namespace AdventOfCode.Utils;

using System.Globalization;
using System.Numerics;

public record Point3D<T>(T X, T Y, T Z) where T : INumber<T>, INumberBase<T>
{
    public static readonly Point3D<T> Origin = (T.Zero, T.Zero, T.Zero);

    public override string ToString() => $"{X}|{Y}|{Z}";

    public static implicit operator (T, T, T)(Point3D<T> pt) => (pt.X, pt.Y, pt.Z);
    public static implicit operator Point3D<T>((T, T, T) t) => new(t.Item1, t.Item2, t.Item3);

    public static Point3D<T> operator -(Point3D<T> a, Point3D<T> b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Point3D<T> operator +(Point3D<T> a, Point3D<T> b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Point3D<T> operator *(Point3D<T> a, T b)
        => new(a.X * b, a.Y * b, a.Z * b);
    public static Point3D<T> operator *(T a, Point3D<T> b)
        => b * a;

    public Point3D<TResult> As<TResult>() where TResult : INumber<TResult>, INumberBase<TResult>
        => new Point3D<TResult>(TResult.CreateChecked(X), TResult.CreateChecked(Y), TResult.CreateChecked(Z));

    public T ManhattanDistance(Point3D<T> other)
        => T.Abs(other.X - X) + T.Abs(other.Y - Y) + T.Abs(other.Z - Z);

    public Point3D<T> MapComponents(Func<T, int, T> selector)
        => new(selector(X, 0), selector(Y, 1), selector(Z, 2));

    public void Deconstruct(out Point2D<T> pt, out T z)
        => (pt, z) = ((X, Y), Z);

    public static IEnumerable<Point3D<T>> Range(Point3D<T> min, Point3D<T> max)
    {
        (var minX, var maxX) = min.X < max.X ? (min.X, max.X) : (max.X, min.X);
        (var minY, var maxY) = min.Y < max.Y ? (min.Y, max.Y) : (max.Y, min.Y);
        (var minZ, var maxZ) = min.Z < max.Z ? (min.Z, max.Z) : (max.Z, min.Z);
        for (T x = minX; x <= maxX; x++)
            for (T y = minY; y <= maxY; y++)
                for (T z = minZ; z <= maxZ; z++)
                    yield return new Point3D<T>(x, y, z);
    }

    public static Func<Point3D<T>, bool> InBounds(Point3D<T> min, Point3D<T> max)
    {
        var (minX, maxX) = min.X < max.X ? (min.X, max.X) : (max.X, min.X);
        var (minY, maxY) = min.Y < max.Y ? (min.Y, max.Y) : (max.Y, min.Y);
        var (minZ, maxZ) = min.Z < max.Z ? (min.Z, max.Z) : (max.Z, min.Z);
        return (pt) =>
            pt.X >= minX && pt.X <= maxX &&
            pt.Y >= minY && pt.Y <= maxY &&
            pt.Z >= minZ && pt.Z <= maxZ;
    }

    public IEnumerable<Point3D<T>> Neighbours()
    {
        yield return this + (T.Zero, T.Zero, T.One);
        yield return this - (T.Zero, T.Zero, T.One);
        yield return this + (T.Zero, T.One, T.Zero);
        yield return this - (T.Zero, T.One, T.Zero);
        yield return this + (T.One, T.Zero, T.Zero);
        yield return this - (T.One, T.Zero, T.Zero);
    }

    public static Point3D<T> Parse(string s)
    {
        var parts = s.Split(',');
        var fp = CultureInfo.InvariantCulture;
        return new Point3D<T>(T.Parse(parts[0], fp), T.Parse(parts[1], fp), T.Parse(parts[2], fp));
    }

    public static (Point3D<T> Min, Point3D<T> Max) Bounds(IEnumerable<Point3D<T>> points)
    {
        var (minX, maxX) = points.MinMax(p => p.X);
        var (minY, maxY) = points.MinMax(p => p.Y);
        var (minZ, maxZ) = points.MinMax(p => p.Z);
        return (new Point3D<T>(minX, minY, minZ), new Point3D<T>(maxX, maxY, maxZ));
    }
}
