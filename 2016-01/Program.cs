using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2016_01
{
    class Program : ProgramBase
    {
        const string PuzzleInput =
                "L3, R2, L5, R1, L1, L2, L2, R1, R5, R1, L1, L2, R2, R4, L4, L3, " +
                "L3, R5, L1, R3, L5, L2, R4, L5, R4, R2, L2, L1, R1, L3, L3, R2, " +
                "R1, L4, L1, L1, R4, R5, R1, L2, L1, R188, R4, L3, R54, L4, R4, R74, " +
                "R2, L4, R185, R1, R3, R5, L2, L3, R1, L1, L3, R3, R2, L3, L4, R1, " +
                "L3, L5, L2, R2, L1, R2, R1, L4, R5, R4, L5, L5, L4, R5, R4, L5, " +
                "L3, R4, R1, L5, L4, L3, R5, L5, L2, L4, R4, R4, R2, L1, L3, L2, " +
                "R5, R4, L5, R1, R2, R5, L2, R4, R5, L2, L3, R3, L4, R3, L2, R1, " +
                "R4, L5, R1, L5, L3, R4, L2, L2, L5, L5, R5, R2, L5, R1, L3, L2, " +
                "L2, R3, L3, L4, R2, R3, L1, R2, L5, L3, R4, L4, R4, R3, L3, R1, " +
                "L3, R5, L5, R1, R5, R3, L1";

        static void Main(string[] args) => new Program().Run();

        private IEnumerable<(int x, int y)> WalkDirections(string directions)
        {
            int x = 0, y = 0;
            int direction = 0;
            yield return (x, y);

            foreach(var d in directions.Split(",").Select(s => s.Trim()))
            {
                var dist = int.Parse(d.Substring(1));
                var turn = (d[0] == 'L') ? -1 : 1;
                direction = (direction + turn) % 4;

                var mod = ((direction & 2) == 0) ? -1 : 1;

                for (int i = 0; i < dist; i++)
                {
                    if ((direction % 2) == 0)
                        x += mod;
                    else
                        y += mod;

                    yield return (x, y);
                }
            }
        }

        private long GetFinalDistance(string directions)
        {
            var final = WalkDirections(directions).Last();
            return Math.Abs(final.x) + Math.Abs(final.y);
        }

        private long GetDistanceToFirstPlaceVisitedTwice(string directions)
        {
            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            foreach(var pt in WalkDirections(directions))
            {
                if (visited.Contains(pt))
                    return Math.Abs(pt.x) + Math.Abs(pt.y);
                visited.Add(pt);
            }
            Error("No point visited twice");
            return 0;
        }

        protected override long? Part1()
        {
            Assert(GetFinalDistance("R2, L3"), 5);
            Assert(GetFinalDistance("R2, R2, R2"), 2);
            Assert(GetFinalDistance("R5, L5, R5, R3"), 12);
            return GetFinalDistance(PuzzleInput);
        }

        protected override long? Part2()
        {
            Assert(GetDistanceToFirstPlaceVisitedTwice("R8, R4, R4, R8"), 4);
            return GetDistanceToFirstPlaceVisitedTwice(PuzzleInput);
        }
    }
}
