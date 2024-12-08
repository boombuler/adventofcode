namespace AdventOfCode._2024;

using Point = Point2D<int>;

class Day08 : Solution
{
    private long CountAntiNodes(string input, Func<IEnumerable<Point>, IEnumerable<Point>> filter)
    {
        var map = input.AsMap();
        IEnumerable<Point> Resonate(Point pt, Point freq)
            => filter(pt.Unfold(pt => pt + freq).TakeWhile(map.Contains));
        
        return (
            from antenna in map
            where antenna.Value != '.'
            group antenna.Index by antenna.Value into grp
            from pair in grp.CombinationPairs()
            let diff = pair.A - pair.B
            from antiNode in Resonate(pair.A, -diff).Concat(Resonate(pair.B, diff))
            select antiNode
        ).Distinct().Count();
    }

    protected override long? Part1() 
    {
        long Solve(string input) => CountAntiNodes(input, x => x.Skip(1).Take(1));

        Assert(Solve(Sample()), 14);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input) => CountAntiNodes(input, Functional.Identity);

        Assert(Solve(Sample()), 34);
        return Solve(Input);
    }
}
