namespace AdventOfCode._2018;

using Point = Point2D<int>;
class Day11 : Solution<Point, string>
{
    const int GRIDSIZE = 300;

    private static Func<Point, Point, long> GetPowerLevelSumLookup(string serial)
    {
        var sums = new long[GRIDSIZE + 1, GRIDSIZE + 1];

        foreach (var pt in Point.Range((1, 1), (GRIDSIZE, GRIDSIZE)))
        {
            var rackId = pt.X + 10;
            var powerLevel = ((rackId * pt.Y) + long.Parse(serial)) * rackId / 100 % 10 - 5;
            sums[pt.X, pt.Y] = powerLevel + sums[pt.X - 1, pt.Y] + sums[pt.X, pt.Y - 1] - sums[pt.X - 1, pt.Y - 1];
        }

        return (A, B) => sums[B.X, B.Y] + sums[A.X - 1, A.Y - 1] - sums[A.X - 1, B.Y] - sums[B.X, A.Y - 1];
    }

    private static Point FindBestFuelSquare(string serial)
    {
        var lookup = GetPowerLevelSumLookup(serial);
        var (origin, _) = Point.Range((1, 1), (GRIDSIZE - 2, GRIDSIZE - 2)).Select(p => (
            Origin: p,
            Level: lookup(p, p + (2, 2))
        )).OrderByDescending(c => c.Level).First();
        return origin;
    }

    private static string FindBestFuelSquareAndSize(string serial)
    {
        var lookup = GetPowerLevelSumLookup(serial);

        var res = Enumerable.Range(1, GRIDSIZE - 1)
            .SelectMany(size => Point.Range((1, 1), (GRIDSIZE - size, GRIDSIZE - size))
                .Select(p => new
                {
                    Origin = p,
                    Size = size + 1,
                    Level = lookup(p, p + (size, size))
                })
            )
            .OrderByDescending(c => c.Level).First();

        return $"{res.Origin},{res.Size}";
    }

    protected override Point Part1()
    {
        Assert(FindBestFuelSquare("42"), new Point(21, 61));
        Assert(FindBestFuelSquare("18"), new Point(33, 45));
        return FindBestFuelSquare(Input);
    }

    protected override string Part2()
    {
        Assert(FindBestFuelSquareAndSize("18"), "90,269,16");
        Assert(FindBestFuelSquareAndSize("42"), "232,251,12");
        return FindBestFuelSquareAndSize(Input);
    }
}
