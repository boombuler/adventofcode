namespace AdventOfCode.Utils;

using System.Globalization;
using System.Numerics;

public record Point2D<T>(T X, T Y) : IComparable<Point2D<T>> where T : INumber<T>, INumberBase<T>
{
    public static readonly Point2D<T> Origin = (T.Zero, T.Zero);
    public static readonly Point2D<T> Up = (T.Zero, -T.One);
    public static readonly Point2D<T> Down = (T.Zero, T.One);
    public static readonly Point2D<T> Left = (-T.One, T.Zero);
    public static readonly Point2D<T> Right = (T.One, T.Zero);

    public override string ToString() => $"{X},{Y}";

    public static implicit operator (T, T)(Point2D<T> pt) => (pt.X, pt.Y);
    public static implicit operator Point2D<T>((T, T) t) => new(t.Item1, t.Item2);

    public static Point2D<T> operator -(Point2D<T> a)
        => new(-a.X, -a.Y);
    public static Point2D<T> operator -(Point2D<T> a, Point2D<T> b)
        => new(a.X - b.X, a.Y - b.Y);
    public static Point2D<T> operator +(Point2D<T> a, Point2D<T> b)
        => new(a.X + b.X, a.Y + b.Y);
    public static Point2D<T> operator *(Point2D<T> a, T b)
        => new(a.X * b, a.Y * b);
    public static Point2D<T> operator *(T a, Point2D<T> b)
        => b * a;
    public static Point2D<T> operator /(Point2D<T> a, T b)
        => new(a.X / b, a.Y / b);
    public static Point2D<T> operator %(Point2D<T> a, T b)
        => new(a.X % b, a.Y % b);
    public static Point2D<T> operator %(Point2D<T> a, Point2D<T> b)
        => new(a.X % b.X, a.Y % b.Y);

    public IEnumerable<Point2D<T>> Neighbours(bool withDiagonal = false)
    {
        yield return this + Right;
        yield return this + Down;
        yield return this + Up;
        yield return this + Left;
        if (withDiagonal)
        {
            yield return this + Up + Left;
            yield return this + Up + Right;
            yield return this + Down + Left;
            yield return this + Down + Right;
        }
    }

    public T ManhattanDistance(Point2D<T> other)
        => T.Abs(other.X - X) + T.Abs(other.Y - Y);

    public static IEnumerable<Point2D<T>> Range(Point2D<T> min, Point2D<T> max)
    {
        (var minX, var maxX) = min.X < max.X ? (min.X, max.X) : (max.X, min.X);
        (var minY, var maxY) = min.Y < max.Y ? (min.Y, max.Y) : (max.Y, min.Y);
        for (T x = minX; x <= maxX; x++)
            for (T y = minY; y <= maxY; y++)
                yield return new(x, y);
    }

    public int CompareTo(Point2D<T> other)
    {
        var dy = Y - other.Y;
        if (dy == T.Zero)
            return T.Sign(X - other.X);
        return T.Sign(dy);
    }

    public Point3D<T> WithZ(T z) => new(X, Y, z);

    public static Point2D<T> Parse(string s)
    {
        var parts = s.Split(',');
        var fp = CultureInfo.InvariantCulture;
        return new Point2D<T>(T.Parse(parts[0], fp), T.Parse(parts[1], fp));
    }
    
    static readonly T[] fSigns = [-T.One, T.Zero, T.One];
    public Point2D<T> Sign()
    {
        T Sign(T dim) => fSigns[T.Sign(dim) + 1];
        return (Sign(X), Sign(Y));
    }

    public Point2D<T> RotateCW() 
        => (Y, -X);

    public Point2D<T> RotateCCW() 
        => (-Y, X);


    /// <summary>
    /// Maps '<', '>', '^' and 'v' into directions. Any other char returns (0,0)
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static Point2D<T> DirectionFromArrow(char c)
        => c switch
        {
            '^' => Up,
            'v' => Down,
            '<' => Left,
            '>' => Right,
            _ => Origin
        };
}
