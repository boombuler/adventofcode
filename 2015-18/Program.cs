using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2015_18
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private long Generate(string inputFile, int generations, bool keepCornersActive = false)
        {
            var grid = ReadLines(inputFile).Select(l => l.Select(c => c == '#' ? 1 : 0).ToArray()).ToArray();

            IEnumerable<(int r, int c)> Cells()
            {
                for (int r = 0; r < grid.Length; r++)
                {
                    for (int c = 0; c < grid[r].Length; c++)
                        yield return (r, c);
                }
            }

            int Neighbours((int r, int c) cell)
            {
                var cnt = 0;
                for (int iy = Math.Max(0, cell.r - 1); iy <= Math.Min(grid.Length - 1, cell.r + 1); iy++)
                {
                    var row = grid[iy];
                    for (int ix = Math.Max(0, cell.c - 1); ix <= Math.Min(row.Length - 1, cell.c + 1); ix++)
                    {
                        if (ix != cell.c || iy != cell.r)
                        {
                            if ((row[ix] & 1) == 1)
                                cnt++;
                        }
                    }
                }
                return cnt;
            }

            void LightCorners()
            {
                if (keepCornersActive)
                {
                    foreach (var r in new[] { 0, grid.Length - 1 })
                    {
                        grid[r][0] |= 3;
                        grid[r][grid[r].Length - 1] |= 3;
                    }
                }
            }
            LightCorners();

            for (int gen = 0; gen < generations; gen++)
            {
                foreach (var cell in Cells())
                {
                    var n = Neighbours(cell);
                    if ((grid[cell.r][cell.c] == 1 && n == 2) || n == 3)
                        grid[cell.r][cell.c] |= 2;
                }

                LightCorners();

                foreach (var cell in Cells())
                    grid[cell.r][cell.c] >>= 1;
            }

            return grid.SelectMany(x => x).Where(c => c == 1).LongCount();
        }


        protected override long? Part1()
        {
            Assert(Generate("Sample", 4), 4);
            return Generate("Input", 100);
        }

        protected override long? Part2()
        {
            Assert(Generate("Sample", 5, true), 17);
            return Generate("Input", 100, true);
        }
    }
}
