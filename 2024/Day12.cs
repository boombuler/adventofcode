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
        long CountCorners(HashSet<Point> region)
        {
            long sum = 0;
            var corners = Enumerable.Range(0, 3).Scan(new { Up, Right },
                (dirs, _) => new { Up = dirs.Up.RotateCW(), Right = dirs.Right.RotateCW() })
                .ToArray();

            foreach (var p in region)
            {
                bool Contains(Point dir) =>region.Contains(p+dir);
                // Outer Corners:
                sum += corners.Count(d => !Contains(d.Up) && !Contains(d.Right));
                // Inner Corners:
                sum += corners.Count(d => Contains(d.Up) && Contains(d.Right) && !Contains(d.Up + d.Right));
            }
            return sum;
        }

        long Solve(string input) 
            => GetFencePrice(input, CountCorners);

        Assert(Solve(Sample()), 80);
        return Solve(Input);
    }
}
