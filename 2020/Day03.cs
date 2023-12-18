namespace AdventOfCode._2020;

using Point = Point2D<int>;

class Day03 : Solution
{
    public static readonly Point DefaultSlope = new(3, 1);

    public static readonly Point[] AllSlopes =
    [
        (1, 1),
        (3, 1),
        (5, 1),
        (7, 1),
        (1, 2)
    ];

    public static long Navigate(StringMap<bool> map, Point slope)
        => Point.Origin.Unfold(p => p + slope).TakeWhile(p => p.Y < map.Height)
            .Count(p => map[(p.X % map.Width, p.Y)]);

    private static long CheckSlopes(string mapFile, params Point[] slopes)
    {
        var map = mapFile.AsMap(c => c == '#');
        return slopes.Select(s => Navigate(map, s)).Aggregate((a, b) => a * b);
    }

    protected override long? Part1()
    {
        Assert(CheckSlopes(Sample(), DefaultSlope), 7);
        return CheckSlopes(Input, DefaultSlope);
    }

    protected override long? Part2()
    {
        Assert(CheckSlopes(Sample(), AllSlopes), 336);
        return CheckSlopes(Input, AllSlopes);
    }
}
