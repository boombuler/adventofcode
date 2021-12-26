namespace AdventOfCode._2017;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Utils;

class Day14 : Solution
{
    const string SAMPLE = "flqrgnkx";

    private static byte[] KnotHash(string prefix)
    {
        var numbers = Enumerable.Range(0, 256).ToList();
        var lengths = Encoding.ASCII.GetBytes(prefix)
            .Select(b => (int)b)
            .Concat(new int[] { 17, 31, 73, 47, 23 })
            .ToArray();
        int current = 0, skipSize = 0;
        for (int round = 0; round < 64; round++)
        {
            foreach (var len in lengths)
            {
                var rangeEnd = current + len - 1;
                for (int i = 0; i < (len / 2); i++)
                {
                    int a = (current + i) % numbers.Count;
                    int b = (rangeEnd - i) % numbers.Count;
                    (numbers[a], numbers[b]) = (numbers[b], numbers[a]);
                }
                current = (current + len + skipSize++) % numbers.Count;
            }
        }

        return numbers
            .Chunk(16)
            .Select(chunk => (byte)chunk.Aggregate((a, b) => a ^ b))
            .ToArray();
    }

    private static bool[][] BuildGrid(string prefix)
    {
        const int ROW_COUNT = 128;
        var result = new bool[ROW_COUNT][];
        for (int r = 0; r < ROW_COUNT; r++)
        {
            var hash = KnotHash(prefix + "-" + r);
            var row = result[r] = new bool[128];
            for (int i = 0; i < 128; i++)
            {
                var bit = (hash[i / 8] >> (7 - (i % 8))) & 1;
                row[i] = bit == 1;
            }
        }
        return result;
    }

    private static long CountSetBitsInGrid(string prefix)
        => BuildGrid(prefix).Select(row => row.Count(x => x)).Sum();

    private static long CountRegions(string prefix)
    {
        var grid = BuildGrid(prefix);

        void ClearRegion(Point2D pt)
        {
            var open = new Queue<Point2D>();
            open.Enqueue(pt);
            while (open.TryDequeue(out var p))
            {
                if (grid[p.Y][p.X])
                {
                    grid[p.Y][p.X] = false;
                    foreach (var n in p.Neighbours())
                    {
                        if (n.X >= 0 && n.Y >= 0 && n.X < 128 && n.Y < 128)
                            open.Enqueue(n);
                    }
                }
            }
        }

        long result = 0;

        foreach (var pt in Point2D.Range((0, 0), (127, 127)))
        {
            if (grid[pt.Y][pt.X])
            {
                result++;
                ClearRegion(pt);
            }
        }
        return result;
    }

    protected override long? Part1()
    {
        Assert(CountSetBitsInGrid(SAMPLE), 8108);
        return CountSetBitsInGrid(Input);
    }

    protected override long? Part2()
    {
        Assert(CountRegions(SAMPLE), 1242);
        return CountRegions(Input);
    }

}
