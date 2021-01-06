using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2017_11
{
    /* 
          \ n  /
        nw +--+ ne
          /    \
        -+      +-
          \    /
        sw +--+ se
          / s  \
    */
    class Program : ProgramBase
    {
        enum Direction
        {
            n, nw, ne, sw, se, s,
        }
        private static Dictionary<Direction, Point2D> Offsets = new Dictionary<Direction, Point2D>()
        {
            {Direction.n, (0, -2) },
            {Direction.nw, (-1, -1) },
            {Direction.ne, (1, -1) },
            {Direction.sw, (-1, 1) },
            {Direction.se, (1, 1) },
            {Direction.s, (0, 2) },
        };

        static void Main(string[] args) => new Program().Run();

        private (long FinalDistance, long MaxDistance) GetDistance(string moves)
        {
            var src = new Point2D(0, 0);
            var dest = moves.Split(',')
                .Select(Enum.Parse<Direction>)
                .Select(d => Offsets[d])
                .Aggregate(
                    new { Point = src, Dist = 0L }, 
                    (res, pt) => new
                    {
                        Point = res.Point + pt,
                        Dist = Math.Max(res.Dist, src.ManhattanDistance(res.Point + pt))
                    }
                );
            return (src.ManhattanDistance(dest.Point) / 2, dest.Dist / 2);
        }
        protected override long? Part1()
        {
            Assert(GetDistance("ne,ne,ne").FinalDistance, 3);
            Assert(GetDistance("ne,ne,sw,sw").FinalDistance, 0);
            Assert(GetDistance("ne,ne,s,s").FinalDistance, 2);
            Assert(GetDistance("se,sw,se,sw,sw").FinalDistance, 3);
            return GetDistance(ReadLines("Input").First()).FinalDistance;
        }

        protected override long? Part2() => GetDistance(ReadLines("Input").First()).MaxDistance;
    }
}
