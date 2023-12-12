namespace AdventOfCode._2021;

using Point = Point2D<int>;

class Day05 : Solution
{
    class Line(int X1, int Y1, int X2, int Y2)
    {
        public Point P1 { get; } = (X1, Y1);
        public Point P2 { get; } = (X2, Y2);

        public static readonly Func<string, Line> Parse = new Regex(@"(?<X1>\-?\d+),(?<Y1>\-?\d+) -> (?<X2>\-?\d+),(?<Y2>\-?\d+)").ToFactory<Line>();
        public IEnumerable<Point> Range()
        {
            var off = new Point(Math.Sign(P2.X - P1.X), Math.Sign(P2.Y - P1.Y));
            for (var p = P1; p != P2; p += off)
                yield return p;
            yield return P2;
        }
    }

    private long CountOverlapping(Func<Line, bool> predicate)
        => Input.Lines().Select(Line.Parse)
            .Where(predicate)
            .SelectMany(l => l.Range())
            .GroupBy(p => p)
            .Count(g => g.Count() > 1);

    protected override long? Part1() => CountOverlapping(l => l.P1.X == l.P2.X || l.P1.Y == l.P2.Y);

    protected override long? Part2() => CountOverlapping(_ => true);
}
