using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2017_12
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        class Pipe
        {
            public static readonly Func<string, Pipe> Parse = new Regex(@"(?<ID>\d+) <-> ((, )?(?<Children>\d+))+").ToFactory<Pipe>();

            public int ID;
            public int[] Children;
        }

        private IEnumerable<IEnumerable<int>> BuildGroups(string inputFile)
        {
            var pipes = ReadLines(inputFile).Select(Pipe.Parse).ToDictionary(p => p.ID);
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

        private int GetGroupMemberCount(string inputFile, int member)
            => BuildGroups(inputFile).First(grp => grp.Contains(member)).Count();

        protected override long? Part1()
        {
            Assert(GetGroupMemberCount("Sample", 0), 6);
            return GetGroupMemberCount("Input", 0);
        }

        protected override long? Part2()
        {
            Assert(BuildGroups("Sample").Count(), 2);
            return BuildGroups("Input").Count();
        }
    }
}
