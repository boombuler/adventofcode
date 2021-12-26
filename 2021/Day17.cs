namespace AdventOfCode._2021;

using System;
using System.Linq;
using AdventOfCode.Utils;

class Day17 : Solution
{
    private static (long MaxY, int Count) ValidVelocities(string descrp)
    {
        var coords = descrp.Split("x=")[1].Split(", y=").SelectMany(n => n.Split("..").Select(long.Parse)).ToArray();
        var tMin = new Point2D(coords[0], coords[2]);
        var tMax = new Point2D(coords[1], coords[3]);
        var isInBounds = Point2D.InBounds(tMin, tMax);

        var peek = 0L;
        var count = 0;

        var minX = Enumerable.Range(1, (int)tMax.X).FirstOrDefault(x => (x * (x + 1)) / 2 >= tMin.X);
        foreach (var velocity in Point2D.Range((minX, tMin.Y), (tMax.X, -tMin.Y)))
        {
            var v = velocity;
            var pt = Point2D.Origin;

            var max = 0L;
            while (pt.Y >= tMin.Y && pt.X <= tMax.X)
            {
                if (isInBounds(pt))
                {
                    peek = Math.Max(peek, max);
                    count++;
                    break;
                }
                pt += v;
                v -= (Math.Sign(v.X), 1);
                max = Math.Max(max, pt.Y);
            }
        }

        return (peek, count);
    }

    const string SAMPLE = "target area: x=20..30, y=-10..-5";

    protected override long? Part1()
    {
        Assert(ValidVelocities(SAMPLE).MaxY, 45);
        return ValidVelocities(Input).MaxY;
    }

    protected override long? Part2()
    {
        Assert(ValidVelocities(SAMPLE).Count, 112);
        return ValidVelocities(Input).Count;
    }
}
