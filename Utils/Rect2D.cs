namespace AdventOfCode.Utils;

public record Rect2D(Point2D TopLeft, Point2D BottomRight)
{
    public long Width => BottomRight.X - TopLeft.X + 1;
    public long Height => BottomRight.Y - TopLeft.Y + 1;

    public bool Contains(Point2D pt)
        => pt.X >= TopLeft.X && pt.X <= BottomRight.X && pt.Y >= TopLeft.Y && pt.Y <= BottomRight.Y;

    public static Rect2D AABB(params Point2D[] points)
        => AABB((IEnumerable<Point2D>)points);

    public static Rect2D AABB(IEnumerable<Point2D> points)
    {
        long minX = long.MaxValue;
        long minY = long.MaxValue;
        long maxX = long.MinValue;
        long maxY = long.MinValue;
        foreach(var p in points)
        {
            if (p.X < minX) minX = p.X;
            if (p.X > maxX) maxX = p.X;
            if (p.Y < minY) minY = p.Y;
            if (p.Y > maxY) maxY = p.Y;
        }
        return new Rect2D((minX, minY), (maxX, maxY));
    }
}
