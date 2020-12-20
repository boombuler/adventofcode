using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2020_11
{
    class Program : ProgramBase
    {
        
        static void Main(string[] args) => new Program().Run();

        private IEnumerable<bool?> LineOfSight(bool?[][] current, int x, int y)
        {
            bool PosValid(int x, int y)
                => x >= 0 && y >= 0 && y < current.Length && x < current[y].Length;

            bool? Check(int dx, int dy)
            {
                var px = x + dx;
                var py = y + dy;
                while(PosValid(px, py))
                {
                    var val = current[py][px];
                    if (val.HasValue)
                        return val;

                    px += dx;
                    py += dy;
                }
                return null;
            }
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    if (dy != 0 || dx != 0)
                        yield return Check(dx, dy);
        }
        private IEnumerable<bool?> Neighbours(bool?[][] current, int x, int y)
        {
            for (int iy = Math.Max(0, y - 1); iy <= Math.Min(current.Length - 1, y + 1); iy++)
            {
                var row = current[iy];
                for (int ix = Math.Max(0, x - 1); ix <= Math.Min(row.Length - 1, x + 1); ix++)
                {
                    if (ix != x || iy != y)
                        yield return row[ix];
                }
            }
        }

        private delegate IEnumerable<bool?>  GetNeighbours(bool?[][] current, int x, int y);

        long CountStableSeats(string inputFile, GetNeighbours neighbours, int maxNeighbours)
        {
            var current = ReadLines(inputFile).Select(l => l.Select(c => c switch
            {
                '.' => (bool?)null,
                '#' => true,
                _ => false
            }).ToArray()).ToArray();

            
            while (true)
            {
                var next = new bool?[current.Length][];
                bool changed = false;
                for (int ir = 0; ir < current.Length; ir++)
                {
                    var curRow = current[ir];
                    var nextRow = next[ir] = new bool?[curRow.Length];
                    for(int ic = 0; ic < curRow.Length;ic++)
                    {
                        if (!curRow[ic].HasValue)
                            continue;

                        var c = neighbours(current, ic, ir).Where(v => v == true).Count();
                        if (c == 0)
                            nextRow[ic] = true;
                        else if (c >= maxNeighbours)
                            nextRow[ic] = false;
                        else
                            nextRow[ic] = curRow[ic];
                        changed |= curRow[ic] != nextRow[ic];
                    }
                }
                if (!changed)
                {
                    return current.SelectMany(r => r).Where(s => s == true).LongCount();
                }
                current = next;
            }
        }

        protected override long? Part1()
        {
            Assert(CountStableSeats("Sample", Neighbours, 4), 37);
            return CountStableSeats("Input", Neighbours, 4);
        }
        protected override long? Part2()
        {
            Assert(CountStableSeats("Sample", LineOfSight, 5), 26);
            return CountStableSeats("Input", LineOfSight, 5);
        }
    }
}
