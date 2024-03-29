﻿namespace AdventOfCode._2017;

using Point = Point2D<int>;
class Day03 : Solution
{
    private static int GetMDForCell(int number)
    {
        var ring = (int)Math.Ceiling(Math.Sqrt(number));
        if ((ring % 2) == 0)
            ring++;

        var pos = new Point(ring / 2, ring / 2); // Get the last position of the current "ring"
        number -= (ring * ring);

        var directions = new Point[] { Point.Left, Point.Up, Point.Right, Point.Down };
        int dir = 0;
        while (number < 0) // Walk back the circle until the wanted number is reached.
        {
            var mv = Math.Min(ring - 1, -number);
            number += mv;
            pos += directions[dir++] * mv;
        }
        return pos.ManhattanDistance((0, 0));
    }

    private static IEnumerable<Point> WalkSpiral()
    {
        var pos = new Point(1, 0);
        long dirLen = 2;
        yield return pos;

        var directions = new Point[] { Point.Up, Point.Left, Point.Down, Point.Right };
        var lengths = new int[] { -1, 0, 0, 1 };

        while (true)
        {
            for (int iDir = 0; iDir < directions.Length; iDir++)
            {
                var dir = directions[iDir];
                var len = lengths[iDir] + dirLen;
                for (int i = 0; i < len; i++)
                {
                    pos += dir;
                    yield return pos;
                }
            }
            dirLen += 2;
        }
    }

    private static long StressTest(long maxValue)
    {
        var values = new Dictionary<Point, long> { [(0, 0)] = 1 };
        foreach (var pos in WalkSpiral())
        {
            var sum = 0L;
            foreach (var n in pos.Neighbours(withDiagonal: true))
                if (values.TryGetValue(n, out var nVal))
                    sum += nVal;
            if (sum > maxValue)
                return sum;
            values[pos] = sum;
        }
        return -1;
    }

    protected override long? Part1()
    {
        Assert(GetMDForCell(1), 0);
        Assert(GetMDForCell(12), 3);
        Assert(GetMDForCell(23), 2);
        Assert(GetMDForCell(25), 4);
        Assert(GetMDForCell(1024), 31);
        return GetMDForCell(int.Parse(Input));
    }

    protected override long? Part2()
    {
        Assert(StressTest(300), 304);
        Assert(StressTest(700), 747);
        return StressTest(int.Parse(Input));
    }
}
