namespace AdventOfCode._2025;

class Day09 : Solution
{
    private static ImmutableList<Point2D<long>> ParsePoints(string input)
        => input.Lines().Select(Parser.LongPoint2D.MustParse).ToImmutableList();

    private static IEnumerable<Rect2D<long>> IterateRegionsBySize(IEnumerable<Point2D<long>> points)
        => points.CombinationPairs((a, b) => Rect2D<long>.AABB(a, b)).OrderByDescending(r => r.Area);

    protected override long? Part1()
    {
        static long Solve(string input) 
            => IterateRegionsBySize(ParsePoints(input)).First().Area;

        Assert(Solve(Sample()), 50);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input)
        {
            var points = ParsePoints(input);

            var edges = points.Append(points[0]).Pairwise((a, b) => Rect2D<long>.AABB(a, b)).ToList();
            bool Overlap(Rect2D<long> a, Rect2D<long> b)
                => (a.Right > b.Left) && (a.Left < b.Right) &&
                   (a.Top < b.Bottom) && (a.Bottom > b.Top);

            return IterateRegionsBySize(points).First(r => !edges.Any(p => Overlap(r, p))).Area;
        }

        Assert(Solve(Sample()), 24);
        return Solve(Input);
    }
}
