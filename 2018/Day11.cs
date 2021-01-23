using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2018
{
    class Day11 : Solution<Point2D, string>
    {
        const int GRIDSIZE = 300;

        private Func<Point2D, Point2D, long> GetPowerLevelSumLookup(string serial)
        {
            var sums = new long[GRIDSIZE+1, GRIDSIZE+1];
           
            foreach (var pt in Point2D.Range((1, 1), (GRIDSIZE, GRIDSIZE)))
            {
                var rackId = pt.X + 10;
                var powerLevel = ((rackId * pt.Y) + long.Parse(serial)) * rackId / 100 % 10 - 5;
                sums[pt.X, pt.Y] = powerLevel + sums[pt.X - 1, pt.Y] + sums[pt.X, pt.Y - 1] - sums[pt.X - 1, pt.Y - 1];
            }

            return (A, B) => sums[B.X, B.Y] + sums[A.X - 1, A.Y - 1] - sums[A.X - 1, B.Y] - sums[B.X, A.Y - 1];
        }

        private Point2D FindBestFuleSquare(string serial)
        {
            var lookup = GetPowerLevelSumLookup(serial);
            var square = Point2D.Range((1, 1), (GRIDSIZE - 2, GRIDSIZE - 2)).Select(p => (
                Origin: p,
                Level: lookup(p, p+(2, 2))
            )).OrderByDescending(c => c.Level).First();
            return square.Origin;
        }

        private string FindBestFuleSquareAndSize(string serial)
        {
            var lookup = GetPowerLevelSumLookup(serial);

            var res = Enumerable.Range(1, GRIDSIZE-1)
                .SelectMany(size => Point2D.Range((1, 1), (GRIDSIZE - size, GRIDSIZE - size))
                    .Select(p => new { 
                        Origin = p, 
                        Size = size+1, 
                        Level = lookup(p, p + (size, size)) 
                    })
                )
                .OrderByDescending(c => c.Level).First();

            return $"{res.Origin},{res.Size}";
        }

        protected override Point2D Part1()
        {
            Assert(FindBestFuleSquare("42"), new Point2D(21,61));
            Assert(FindBestFuleSquare("18"), new Point2D(33,45));
            return FindBestFuleSquare(Input);
        }

        protected override string Part2()
        {
            Assert(FindBestFuleSquareAndSize("18"), "90,269,16");
            Assert(FindBestFuleSquareAndSize("42"), "232,251,12");
            return FindBestFuleSquareAndSize(Input);
        }
    }
}
