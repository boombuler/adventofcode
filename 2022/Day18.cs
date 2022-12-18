namespace AdventOfCode._2022;

class Day18 : Solution
{
    private long CountSurfacePlanes(string input)
    {
        var droplets = input.Lines().Select(Point3D.Parse).ToHashSet();
        return droplets.SelectMany(d => d.Neighbours()).Count(d => !droplets.Contains(d));
    }

    private long CountExteriorSurfacePlanes(string input)
    {
        var points = input.Lines().Select(Point3D.Parse).ToHashSet();
        var (min, max) = Point3D.Bounds(points);
        min = min - (1, 1, 1);
        max = max + (1, 1, 1);
        var inBounds = Point3D.InBounds(min, max);
        
        var water = new HashSet<Point3D>();
        var open = new Queue<Point3D>();
        open.Enqueue(min);
        while (open.TryDequeue(out var pt))
        {
            if (!water.Add(pt))
                continue;

            foreach (var n in pt.Neighbours().Where(inBounds))
            {
                if (!points.Contains(n))
                    open.Enqueue(n);
            }
        }

        return points.SelectMany(n => n.Neighbours()).Count(water.Contains);
    }

    protected override long? Part1()
    {
        Assert(CountSurfacePlanes(Sample()), 64);
        return CountSurfacePlanes(Input);
    }

    protected override long? Part2()
    {
        Assert(CountExteriorSurfacePlanes(Sample()), 58);
        return CountExteriorSurfacePlanes(Input);
    }
}
