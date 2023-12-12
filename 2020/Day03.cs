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

    public static long Navigate(bool[][] map, Point slope)
    {
        long hits = 0;
        for (var pos = Point.Origin; pos.Y < map.Length; pos += slope)
        {
            if (map[pos.Y][pos.X % map[pos.Y].Length])
                hits++;
        }

        return hits;
    }

    private static bool[][] ReadTreeMap(string mappings)
        => mappings.Lines()
            .Select(s => s.Select(c => c == '#').ToArray())
            .ToArray();

    private static long CheckSlopes(string mapFile, params Point[] slopes)
    {
        var map = ReadTreeMap(mapFile);
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
