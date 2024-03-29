﻿namespace AdventOfCode._2022;

using Point = Point3D<int>;
class Day18 : Solution
{
    private static long CountSurfacePlanes(string input)
    {
        var droplets = input.Lines().Select(Point.Parse).ToHashSet();
        return droplets.SelectMany(d => d.Neighbours()).Count(d => !droplets.Contains(d));
    }

    private static long CountExteriorSurfacePlanes(string input)
    {
        var points = input.Lines().Select(Point.Parse).ToHashSet();
        var (min, max) = Point.Bounds(points);
        min -= (1, 1, 1);
        max += (1, 1, 1);
        var inBounds = Point.InBounds(min, max);

        var water = new HashSet<Point>();
        var open = new Queue<Point>();
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
