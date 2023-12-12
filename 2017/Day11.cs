namespace AdventOfCode._2017;

/* 
      \ n  /
    nw +--+ ne
      /    \
    -+      +-
      \    /
    sw +--+ se
      / s  \
*/
using Point = Point2D<int>;

class Day11 : Solution
{
    enum Direction
    {
        n, nw, ne, sw, se, s,
    }
    private static readonly Dictionary<Direction, Point> Offsets = new()
    {
        {Direction.n, (0, -2) },
        {Direction.nw, (-1, -1) },
        {Direction.ne, (1, -1) },
        {Direction.sw, (-1, 1) },
        {Direction.se, (1, 1) },
        {Direction.s, (0, 2) },
    };

    private static (long FinalDistance, long MaxDistance) GetDistance(string moves)
    {
        var src = new Point(0, 0);
        var dest = Parser.Enum<Direction>().List(',').MustParse(moves)
            .Select(d => Offsets[d])
            .Aggregate(
                new { Point = src, Dist = 0L },
                (res, pt) => new
                {
                    Point = res.Point + pt,
                    Dist = Math.Max(res.Dist, src.ManhattanDistance(res.Point + pt))
                }
            );
        return (src.ManhattanDistance(dest.Point) / 2, dest.Dist / 2);
    }
    protected override long? Part1()
    {
        Assert(GetDistance("ne,ne,ne").FinalDistance, 3);
        Assert(GetDistance("ne,ne,sw,sw").FinalDistance, 0);
        Assert(GetDistance("ne,ne,s,s").FinalDistance, 2);
        Assert(GetDistance("se,sw,se,sw,sw").FinalDistance, 3);
        return GetDistance(Input).FinalDistance;
    }

    protected override long? Part2() => GetDistance(Input).MaxDistance;
}
