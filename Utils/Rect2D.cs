namespace AdventOfCode.Utils;

using System.Numerics;

public record Rect2D<T>(Point2D<T> TopLeft, Point2D<T> BottomRight)
    where T : INumber<T>, INumberBase<T>, IMinMaxValue<T>
{
    public T Width => BottomRight.X - TopLeft.X + T.One;
    public T Height => BottomRight.Y - TopLeft.Y + T.One;

    public T Area => Width * Height;
    public T Left => TopLeft.X;
    public T Right => BottomRight.X;
    public T Top => TopLeft.Y;
    public T Bottom => BottomRight.Y;

    public bool Contains(Point2D<T> pt)
        => pt.X >= TopLeft.X && pt.X <= BottomRight.X && pt.Y >= TopLeft.Y && pt.Y <= BottomRight.Y;

    public static Rect2D<T> AABB(params Point2D<T>[] points)
        => AABB((IEnumerable<Point2D<T>>)points);

    public static Rect2D<T> AABB(IEnumerable<Point2D<T>> points)
    {
        T minX = T.MaxValue;
        T minY = T.MaxValue;
        T maxX = T.MinValue;
        T maxY = T.MinValue;
        foreach (var p in points)
        {
            if (p.X < minX)
                minX = p.X;
            if (p.X > maxX)
                maxX = p.X;
            if (p.Y < minY)
                minY = p.Y;
            if (p.Y > maxY)
                maxY = p.Y;
        }
        return new Rect2D<T>((minX, minY), (maxX, maxY));
    }
}
