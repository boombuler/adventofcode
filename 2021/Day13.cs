namespace AdventOfCode._2021;

using Point = Point2D<int>;

class Day13 : Solution<int?, string>
{
    record Command(bool Horizontal, int Offset);

    private HashSet<Point> Fold(HashSet<Point> points, Command cmd)
    {
        int NewPos(int old) => old > cmd.Offset ? cmd.Offset - (old - cmd.Offset) : old;

        if (cmd.Horizontal)
            return points.Select(p => new Point(p.X, NewPos(p.Y))).ToHashSet();
        return points.Select(p => new Point(NewPos(p.X), p.Y)).ToHashSet();
    }

    private static (HashSet<Point> Points, IEnumerable<Command> Commands) Parse(string input)
        => (
            input.Lines().TakeWhile(l => !string.IsNullOrEmpty(l)).Select(l => l.Split(',')).Select(l => new Point(int.Parse(l[0]), int.Parse(l[1]))).ToHashSet(),
            input.Lines().SkipWhile(l => !string.IsNullOrEmpty(l)).Skip(1).Select(l => l.Split(' ').Last().Split('=')).Select(l => new Command(l[0] == "y", int.Parse(l[1])))
        );

    private int TestFold(string input)
    {
        var (pts, commands) = Parse(input);
        return Fold(pts, commands.First()).Count;
    }

    protected override int? Part1()
    {
        Assert(TestFold(Sample()), 17);
        return TestFold(Input);
    }

    protected override string Part2()
    {
        var (pts, commands) = Parse(Input);
        pts = commands.Aggregate(pts, Fold);
        return new OCR6x5().Decode((x, y) => pts.Contains((x, y)), pts.Max(p => (int)p.X) + 1);
    }
}
