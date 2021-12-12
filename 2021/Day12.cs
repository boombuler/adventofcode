using AdventOfCode.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AdventOfCode._2021
{
    class Day12 : Solution
    {
        const string START = "start";
        const string END = "end";
        long CountUniquePaths(string map, int smallCaveRevisitCount = 0)
        {
            var edges = map.Lines().Select(l => l.Split('-'))
                .SelectMany(l => new[] { (A: l[0], B: l[1]), (A: l[1], B: l[0]) })
                .Where(n => n.B != START) // don't revisit start cave
                .ToLookup(n => n.A, n => n.B);
            var smallCaves = edges.Select(e => e.Key).Where(k => char.IsLower(k.First())).ToHashSet();
            var paths = new HashSet<ImmutableList<string>>();
            var open = new Queue<(int SmallCavesRevisited, ImmutableList<string> Path)>();
            open.Enqueue((0, ImmutableList<string>.Empty.Add(START)));
            while(open.TryDequeue(out var current))
            {
                foreach(var segment in edges[current.Path.Last()])
                {
                    var count = current.SmallCavesRevisited + ((smallCaves.Contains(segment) && current.Path.Contains(segment)) ? + 1 : 0);
                    if (count > smallCaveRevisitCount)
                        continue;
                    var way = current.Path.Add(segment);
                    if (segment == END)
                        paths.Add(way);
                    else
                        open.Enqueue((count, way));
                }
            }
            return paths.Count;
        }

        protected override long? Part1()
        {
            Assert(CountUniquePaths(Sample("1")), 10);
            Assert(CountUniquePaths(Sample("2")), 19);
            Assert(CountUniquePaths(Sample("3")), 226);
            return CountUniquePaths(Input);
        }

        protected override long? Part2()
        {
            Assert(CountUniquePaths(Sample("1"), 1), 36);
            Assert(CountUniquePaths(Sample("2"), 1), 103);
            Assert(CountUniquePaths(Sample("3"), 1), 3509);
            return CountUniquePaths(Input, 1);
        }
    }
}
