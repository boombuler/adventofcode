using AdventOfCode.Utils;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2021
{
    class Day15 : Solution
    {
        private long? FindPath(string input, int expandMap = 1)
        {
            var baseMap = input.Cells(c => c - '0');
            var size = baseMap.Keys.Max() + (1,1);
            var riskMap = new Dictionary<Point2D, long>();
            
            foreach(var of in Point2D.Range(Point2D.Origin, (expandMap - 1, expandMap - 1)))
            {
                var offset = new Point2D(of.X * size.X, of.Y * size.Y);
                foreach (var (k, v) in baseMap)
                    riskMap[k + offset] = (of.X + of.Y + v + 1) % 9 + 1;
            }

            var target = riskMap.Keys.Max();

            var open = new MinHeap<(long Cost, Point2D Path)>(ComparerBuilder<(long Cost, Point2D Path)>.CompareBy(i => i.Cost));
            open.Push((0, Point2D.Origin));
            var visited = new HashSet<Point2D>();
            while(open.TryPop(out var cur))
            {
                var (curCost, curPath) = cur;
                if (curPath == target)
                    return curCost;
                if (!visited.Add(curPath))
                    continue;
                foreach(var n in curPath.Neighbours())
                {
                    if (riskMap.TryGetValue(n, out var cost))
                        open.Push((curCost + cost, n));
                }
            }
            return null;
        }

        protected override long? Part1()
        {
            Assert(FindPath(Sample()), 40);
            return FindPath(Input);
        }

        protected override long? Part2()
        {
            Assert(FindPath(Sample(), 5), 315);
            return FindPath(Input, 5);
        }
    }
}
