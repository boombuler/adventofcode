namespace AdventOfCode._2024;

using Point = Point2D<int>;
using static Point2D<int>;
class Day12 : Solution
{
    private long GetFencePrice(string input, Func<HashSet<Point>, long> getPerimeter)
    {
        var map = input.AsMap();
        var seen = new HashSet<Point>();

        long sum = 0;
        foreach(var (index, c) in map)
        {
            if (seen.Contains(index))
                continue;

            var region = new HashSet<Point>();
            var open = new Queue<Point>([index]);
            while(open.TryDequeue(out var p))
            {
                if (map[p] != c || !seen.Add(p))
                    continue;
                region.Add(p);
                foreach (var p2 in p.Neighbours().Where(map.Contains))
                    open.Enqueue(p2);
            }

            sum += region.Count * getPerimeter(region);
        }

        return sum;
    }

    protected override long? Part1() 
    {
        long Solve(string input)
            => GetFencePrice(input, region => region.SelectMany(p => p.Neighbours()).Count(p => !region.Contains(p)));

        Assert(Solve(Sample()), 140);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long CountCorners(HashSet<Point> region) => (
            from pos in region
            from corner in pos + [Origin, Right, Down, Right + Down]
            group (corner - pos) by corner into g
            let aggr = g.Aggregate((Even: true, Delta: (Right+Down)), (acc, p) => (!acc.Even, acc.Delta-p))
            select aggr switch
            {
                (false, _) => 1, // Odd number of adjacent blocks -> 1 corner
                (_, (0,0)) => 2, // Special case: Two opposing corners
                _ => 0
            }
        ).Sum();

        long Solve(string input) 
            => GetFencePrice(input, CountCorners);

        Assert(Solve(Sample()), 80);
        return Solve(Input);
    }
}
