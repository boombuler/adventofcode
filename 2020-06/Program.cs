using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2020_06
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private IEnumerable<IEnumerable<string>> ReadGroups(string inputFile)
        {
            List<string> grp = new List<string>();
            foreach (var line in ReadLines(inputFile))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    yield return grp;
                    grp = new List<string>();
                }
                else
                    grp.Add(line);
            }
            if (grp.Count > 0)
                yield return grp;
        }

        private int CollectUniqueResults(string inputFile)
            => ReadGroups(inputFile)
                .Select(grp => grp.SelectMany(s => s).Distinct().Count())
                .Sum();
        
        private int CollectGroupResults(string inputFile)
            => ReadGroups(inputFile)
                .Select(grp => grp.SelectMany(s => s)
                    .GroupBy(c=>c)
                    .Where(g => g.Count() == grp.Count())
                    .Count()
                ).Sum();

        protected override long? Part1()
        {
            Assert(CollectUniqueResults("SampleInput"), 11);
            return CollectUniqueResults("Input");
        }

        protected override long? Part2()
        {
            Assert(CollectGroupResults("SampleInput"), 6);
            return CollectGroupResults("Input");
        }
    }
}
