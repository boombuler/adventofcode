namespace AdventOfCode._2024;

using static Parser;
using Point = Point2D<long>;

class Day14 : Solution
{
    private readonly Point InputMapSize = (101, 103);
    private static readonly Parser<(Point Point, Point Velocity)[]> InputParser = (
        from p in "p=" + LongPoint2D
        from v in "v=" + LongPoint2D
        select (p, v)
    ).List(NL);

    private static Func<int, IEnumerable<Point>> GetRobotFactory(string input, Point mapSize)
    {
        var lst = InputParser.MustParse(input);
        return (time) => lst.Select(r => MathExt.Mod(r.Point + (r.Velocity * time), mapSize));
    }

    private static long GetSafetyFactor(string input, Point mapSize, int seconds = 100)
        => GetRobotFactory(input, mapSize)(seconds).Select(p => (p - mapSize / 2) switch
            {
                ( > 0, > 0) => 1,
                ( > 0, < 0) => 2,
                ( < 0, > 0) => 3,
                ( < 0, < 0) => 4,
                _ => -1
            }).Where(n => n != -1).CountBy(n => n).Aggregate(1, (a, b) => a * b.Value);
    
    protected override long? Part1() 
    {
        Assert(GetSafetyFactor(Sample(), (7, 11)), 12);
        return GetSafetyFactor(Input, InputMapSize);
    }

    protected override long? Part2()
    {
        var robots = GetRobotFactory(Input, InputMapSize);

        var variances = (
            from i in Enumerable.Range(0, (int)Math.Max(InputMapSize.X, InputMapSize.Y))
            let pts = robots(i).ToList()
            let avg = pts.Sum() / pts.Count
            select pts.Select(p => p - avg)
                .Select(p => new Point(p.X * p.X, p.Y * p.Y))
                .Sum()
        ).Select((n, i) => new { n.X, n.Y, Index = i }).ToList();

        return MathExt.ChineseRemainder(
            (variances.MinBy(n => n.X)!.Index, InputMapSize.X),
            (variances.MinBy(n => n.Y)!.Index, InputMapSize.Y)
        ).a;
    }
}
