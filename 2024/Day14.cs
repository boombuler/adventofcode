namespace AdventOfCode._2024;

using static Parser;
using Point = Point2D<long>;

class Day14 : Solution
{
    private static readonly Parser<(Point Point, Point Velocity)> InputParser =
        from p in "p=" + LongPoint2D
        from v in "v=" + LongPoint2D
        select (p, v);
    private readonly Point InputMapSize = (101, 103);

    private long GetSafetyFactor(string input, Point mapSize, int seconds = 100)
    {
        var mid = mapSize / 2;
        return input.Lines().Select(InputParser.MustParse)
            .Select(r => MathExt.Mod(r.Point + (r.Velocity * seconds), mapSize))
            .Select(p => (p - mid) switch
            {
                ( > 0, > 0) => 1,
                ( > 0, < 0) => 2,
                ( < 0, > 0) => 3,
                ( < 0, < 0) => 4,
                _ => -1
            }).Where(n => n != -1).CountBy(n => n).Aggregate(1, (a, b) => a * b.Value);
    }
    
    protected override long? Part1() 
    {
        Assert(GetSafetyFactor(Sample(), (7, 11)), 12);
        return GetSafetyFactor(Input, InputMapSize);
    }

    protected override long? Part2()
    {
        var robots = Input.Lines().Select(InputParser.MustParse).ToList();

        for (int i = 0; true; i++)
        {
            var seen = new HashSet<Point>();
            var anyRobotStacked = robots
                .Select(r => MathExt.Mod(r.Point + (r.Velocity * i), InputMapSize))
                .Any(p => !seen.Add(p));
            if (!anyRobotStacked)
                return i;
        }
    }
}
