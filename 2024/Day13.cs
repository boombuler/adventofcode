namespace AdventOfCode._2024;

using static Parser;
using Point = Point2D<long>;

class Day13 : Solution
{
    private static readonly Parser<Point> PointParser =
        from prefix in Any.Until(": X").ThenL(Any)
        from x in (Long + ", Y").ThenL(Any)
        from y in Long + "\n"
        select new Point(x, y);

    private long GetPrizeCost(Point A, Point B, Point Prize)
    {
        // Prize.X = a*A.X + b*B.X
        // Prize.Y = a*A.Y + b*B.Y
        var det = A.X * B.Y - A.Y * B.X;
        if (det == 0)
            return 0;

        var m = (Prize.X * B.Y - Prize.Y * B.X);
        var n = (Prize.Y * A.X - Prize.X * A.Y);
        if (m % det != 0 || n % det != 0)
            return 0;

        return (3*m+n) / det;
    }
    
    private long GetTotalCost(string input, Point offset)
        => PointParser.Take(3).List("\n").MustParse(input + "\n")
            .Sum(n => GetPrizeCost(n[0], n[1], n[2] + offset));

    protected override long? Part1() 
    {
        Assert(GetTotalCost(Sample(), Point.Origin), 480);
        return GetTotalCost(Input, Point.Origin);
    }

    protected override long? Part2()
        => GetTotalCost(Input, (10000000000000L, 10000000000000L));
}
