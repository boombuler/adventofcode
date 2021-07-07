using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2018
{
    class Day25 : Solution<int?>
    {
        public record Coord(int X, int Y, int Z, int T)
        {
            public static readonly Func<string, Coord> Parse = new Regex(@"(?<X>-?\d+),(?<Y>-?\d+),(?<Z>-?\d+),(?<T>-?\d+)").ToFactory<Coord>();

            public bool InRangeOf(Coord other)
                => (Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z) + Math.Abs(other.T - T)) <= 3;
        }

        private int CountConstellation(string input)
        {
            var points = input.Lines().Select(Coord.Parse).ToList();
            var groups = points.ToDictionary(p => p, p => new HashSet<Coord>() { p });
            var constellations = groups.Values.ToHashSet();

            foreach(var (a, (b, _)) in points.Combinations(2))
            {
                if (!a.InRangeOf(b))
                    continue;

                var (grpA, grpB) = (groups[a], groups[b]);
                if (grpA == grpB)
                    continue;

                constellations.Remove(grpB);
                grpA.UnionWith(grpB);
                foreach (var n in grpB)
                    groups[n] = grpA;
            }

            return constellations.Count;
        }

        protected override int? Part1()
        {
            Assert(CountConstellation(Sample("1")), 2);
            Assert(CountConstellation(Sample("2")), 4);
            Assert(CountConstellation(Sample("3")), 3);
            Assert(CountConstellation(Sample("4")), 8);

            return CountConstellation(Input);
        }
    }
}
