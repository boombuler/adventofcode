namespace AdventOfCode._2022;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Day14 : Solution
{
    private long FillCave(string input, Func<Point2D, long, bool> finished)
    {
        var cave = new HashSet<Point2D>(
            from line in input.Lines()
            from range in line.Split(" -> ").Select(Point2D.Parse).SlidingWindow(2)
            from pt in Point2D.Range(range[0], range[1])
            select pt
        );
        var abys = cave.Max(p => p.Y);
        var rocks = cave.Count;
        var directions = new Point2D[] { (0, 1), (-1, 1), (1, 1) };
        while (true)
        {
            var pos = new Point2D(500, 0);
            while (true)
            {
                var dir = directions.FirstOrDefault(d => !cave.Contains(d + pos), Point2D.Origin);
                if (dir == Point2D.Origin)
                {
                    cave.Add(pos);
                    if (finished(pos, abys))
                        return cave.Count - rocks;
                        
                    break;
                }
                pos += dir;
                if (pos.Y >= (abys+2))
                {
                    cave.Add(pos);
                    rocks++;
                    break;
                }
            }
        }
    }


    protected override long? Part1()
    {
        long Solve(string input) =>
            FillCave(input, (pt, y) => pt.Y >= y) - 1;

        Assert(Solve(Sample()), 24);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input)
            => FillCave(input, (pt, _) => pt.Y == 0);

        Assert(Solve(Sample()), 93);
        return Solve(Input);
    }
}
