using AdventOfCode.Utils;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2021
{
    class Day20 : Solution
    {
        public long Enhance(string input, int rounds)
        {
            var lookup = input.Lines().First().Select((c, i) => new { Value = c == '#', Index = i }).Where(x => x.Value).Select(x => x.Index).ToHashSet();
            var img = string.Join("\n", input.Lines().Skip(2)).Cells(c => c == '#').Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToHashSet();
            var topLeft = new Point2D(0, 0);
            var bottomRight = new Point2D(input.Lines().Skip(2).First().Length-1, input.Lines().Skip(2).Count()-1);
            bool everythingLit = false;

            for (int i = 0; i < rounds; i++)
            {
                var next = new HashSet<Point2D>();
                var inCurrentImg = Point2D.InBounds(topLeft, bottomRight);

                topLeft -= (1, 1);
                bottomRight += (1, 1);
                
                foreach (var px in Point2D.Range(topLeft, bottomRight))
                {
                    var window = from y in Enumerable.Range((int)px.Y - 1, 3)
                                 from x in Enumerable.Range((int)px.X - 1, 3)
                                 select new Point2D(x, y);

                    var lookupIdx = window.Select((p, i) => inCurrentImg(p) ? img.Contains(p) : everythingLit)
                        .Select((p,i) => (p ? 1 : 0) << (8 - i)).Aggregate((a, b) => a | b);
                    if (lookup.Contains(lookupIdx))
                        next.Add(px);
                }
                everythingLit ^= lookup.Contains(0);
                img = next;
            }
            return everythingLit ? long.MaxValue : img.Count;
        }

        protected override long? Part1()
        {
            Assert(Enhance(Sample(), 2), 35);
            return Enhance(Input, 2);
        }

        protected override long? Part2()
        {
            Assert(Enhance(Sample(), 50), 3351);
            return Enhance(Input, 50);
        }
    }
}
