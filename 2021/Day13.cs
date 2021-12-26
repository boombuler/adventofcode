namespace AdventOfCode._2021;

using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day13 : Solution<long?, string>
{
    record Command(bool Horizontal, long Offset);

    private HashSet<Point2D> Fold(HashSet<Point2D> points, Command cmd)
    {
        long NewPos(long old) => old > cmd.Offset ? cmd.Offset - (old - cmd.Offset) : old;

        if (cmd.Horizontal)
            return points.Select(p => new Point2D(p.X, NewPos(p.Y))).ToHashSet();
        return points.Select(p => new Point2D(NewPos(p.X), p.Y)).ToHashSet();
    }

    private static (HashSet<Point2D> Points, IEnumerable<Command> Commands) Parse(string input)
        => (
            input.Lines().TakeWhile(l => !string.IsNullOrEmpty(l)).Select(l => l.Split(',')).Select(l => new Point2D(long.Parse(l[0]), long.Parse(l[1]))).ToHashSet(),
            input.Lines().SkipWhile(l => !string.IsNullOrEmpty(l)).Skip(1).Select(l => l.Split(' ').Last().Split('=')).Select(l => new Command(l[0] == "y", long.Parse(l[1])))
        );

    private long TestFold(string input)
    {
        var (pts, commands) = Parse(input);
        return Fold(pts, commands.First()).Count;
    }

    protected override long? Part1()
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
