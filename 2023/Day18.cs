namespace AdventOfCode._2023;

using static Parser;
using static Point2D<long>;
class Day18 : Solution
{
    record Instruction(Point2D<long> Direction, int Count);
    
    private static readonly Func<string, Instruction[][]> ParseInput = (
        from d1 in AnyChar("RDLU", [Right, Down, Left, Up])
        from n1 in Int.Token()
        from n2 in "(#" + HexDigit.Take(5).Text().Select(n => Convert.ToInt32(n, 16))
        from d2 in AnyChar("0123", [Right, Down, Left, Up]) + ")"
        select new Instruction[] { new (d1, n1), new (d2, n2) }
    ).List('\n');

    private static long GetTotalArea(string input, int idx)
        => ParseInput(input)
            .Scan(Origin, (a, i) => a + i[idx].Direction * i[idx].Count)
            .SlidingWindow(2)
            .Sum(pts => pts[0].ManhattanDistance(pts[1]) + pts[0].X * pts[1].Y - pts[0].Y * pts[1].X) / 2 + 1;

    protected override long? Part1()
    {
        Assert(GetTotalArea(Sample(), 0), 62);
        return GetTotalArea(Input, 0);
    }

    protected override long? Part2()
    {
        Assert(GetTotalArea(Sample(), 1), 952408144115);
        return GetTotalArea(Input, 1);
    }
}
