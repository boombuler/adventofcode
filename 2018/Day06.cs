using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode._2018
{
    class Day06 : Solution
    {
        private static readonly Func<string, Point2D> ParsePt = new Regex(@"(?<X>\d+), (?<Y>\d+)", RegexOptions.Compiled).ToFactory<Point2D>();

        private int FindLargestArea(string input)
        {
            var points = input.Lines().Select(ParsePt).ToList();
            var (maxX, maxY) = (points.Max(p => p.X), points.Max(p => p.Y));

            var itemCounts = new int[points.Count];
            var unlimited = new HashSet<int>();
            foreach (var loc in Point2D.Range(Point2D.Origin, (maxX, maxY)))
            {
                var near = points
                    .Select((p, i) => new { Distance = p.ManhattanDistance(loc), Index = i })
                    .GroupBy(x => x.Distance)
                    .OrderBy(x => x.Key)
                    .First();

                if (near.Count() > 1)
                    continue;
                var idx = near.First().Index;
                itemCounts[idx]++;
                if (loc.X == 0 || loc.X == maxX || loc.Y == 0 || loc.Y == maxY)
                    unlimited.Add(idx);
            }
            return Enumerable.Range(0, points.Count).Except(unlimited).Max(i => itemCounts[i]);
        }
        protected override long? Part1()
        {
            Assert(FindLargestArea(Sample()), 17);
            return FindLargestArea(Input);
        }

        protected override long? Part2()
        {
            var points = Input.Lines().Select(ParsePt).ToList();
            var result = 0;
            foreach (var loc in Point2D.Range(Point2D.Origin, (points.Max(p => p.X), points.Max(p => p.Y))))
            {
                if (points.Select(p => p.ManhattanDistance(loc)).Sum() < 10000)
                    result++;
            }
            return result;
        }
    }
}
