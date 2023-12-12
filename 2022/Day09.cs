namespace AdventOfCode._2022;

using Point = Point2D<int>;

class Day09 : Solution
{
    private static IEnumerable<Point> ParseMoves(string input)
        => from line in input.Lines()
           let parts = line.Split(' ')
           from m in Enumerable.Repeat(parts[0] switch
           {
               "U" => new Point(0, -1),
               "D" => new Point(0, +1),
               "L" => new Point(-1, 0),
               _ => new Point(+1, 0),
           }, int.Parse(parts[1]))
           select m;

    private static int CountUniqueLocations(string input, int knotCount)
    {
        var visited = new HashSet<Point>();
        var knots = Enumerable.Repeat(Point.Origin, knotCount).ToArray();
        double Length(Point p) => Math.Sqrt(p.X * p.X + p.Y * p.Y);

        foreach (var mv in ParseMoves(input))
        {
            knots[0] += mv;
            for (int i = 1; i < knotCount; i++)
            {
                var d = knots[i - 1] - knots[i];
                if (Length(d) >= 2)
                    knots[i] += (Math.Sign(d.X), Math.Sign(d.Y));
                else
                    break;
            }
            visited.Add(knots[knotCount - 1]);
        }
        return visited.Count;
    }

    protected override long? Part1()
    {
        const int KnotCount = 2;
        Assert(CountUniqueLocations(Sample("1"), KnotCount), 13);
        return CountUniqueLocations(Input, KnotCount);
    }

    protected override long? Part2()
    {
        const int KnotCount = 10;
        Assert(CountUniqueLocations(Sample("1"), KnotCount), 1);
        Assert(CountUniqueLocations(Sample("2"), KnotCount), 36);
        return CountUniqueLocations(Input, KnotCount);
    }
}
