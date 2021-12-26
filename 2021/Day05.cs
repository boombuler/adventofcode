namespace AdventOfCode._2021;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day05 : Solution
{
    class Line
    {
        public Point2D P1 { get; }
        public Point2D P2 { get; }

        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by factory (reflection)")]
        private Line(long X1, long Y1, long X2, long Y2) => (P1, P2) = ((X1, Y1), (X2, Y2));

        public static readonly Func<string, Line> Parse = new Regex(@"(?<X1>\-?\d+),(?<Y1>\-?\d+) -> (?<X2>\-?\d+),(?<Y2>\-?\d+)").ToFactory<Line>();
        public IEnumerable<Point2D> Range()
        {
            var off = new Point2D(Math.Sign(P2.X - P1.X), Math.Sign(P2.Y - P1.Y));
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
