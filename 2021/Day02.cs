namespace AdventOfCode._2021;

using Point = Point3D<int>;

class Day02 : Solution
{
    private long RunCommands(Func<Point, string, Point> getOffset)
    {
        var pt = Input.Lines()
            .Select(l => l.Split(' '))
            .Select(line => new { Dir = line[0], Amount = int.Parse(line[1]) })
            .Aggregate(Point.Origin, (cur, cmd) => cur + cmd.Amount * getOffset(cur, cmd.Dir));
        return pt.X * pt.Y;
    }

    protected override long? Part1()
        => RunCommands((_, dir) => dir switch
        {
            "up" => new Point(0, -1, 0),
            "down" => new Point(0, 1, 0),
            _ => new Point(1, 0, 0)
        });

    protected override long? Part2()
        => RunCommands((cur, dir) => dir switch
        {
            "up" => new Point(0, 0, -1),
            "down" => new Point(0, 0, 1),
            _ => new Point(1, cur.Z, 0)
        });
}
