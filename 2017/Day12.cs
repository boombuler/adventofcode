using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2017
{
    class Day12 : Solution
    {
        class Pipe
        {
            public static readonly Func<string, Pipe> Parse = new Regex(@"(?<ID>\d+) <-> ((, )?(?<Children>\d+))+").ToFactory<Pipe>();

            public int ID;
            public int[] Children;
        }

        private IEnumerable<IEnumerable<int>> BuildGroups(string input)
        {
            var pipes = input.Lines().Select(Pipe.Parse).ToDictionary(p => p.ID);
            var group = new HashSet<int>();
            var open = new Queue<int>();
            while (pipes.Count > 0)
            {
                open.Enqueue(pipes.Keys.First());
                while (open.TryDequeue(out var m))
                {
                    if (!group.Add(m))
                        continue;
                    var pipe = pipes[m];
                    pipes.Remove(m);
                    foreach (var id in pipe.Children)
                        open.Enqueue(id);
                }
                yield return group;
                group = new HashSet<int>();
            }
        }

        private int GetGroupMemberCount(string input, int member)
            => BuildGroups(input).First(grp => grp.Contains(member)).Count();

        protected override long? Part1()
        {
            Assert(GetGroupMemberCount(Sample(), 0), 6);
            return GetGroupMemberCount(Input, 0);
        }

        protected override long? Part2()
        {
            Assert(BuildGroups(Sample()).Count(), 2);
            return BuildGroups(Input).Count();
        }
    }
}
