using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2020
{
    class Day06 : Solution
    {
        private IEnumerable<IEnumerable<string>> ReadGroups(string input)
        {
            List<string> grp = new List<string>();
            foreach (var line in input.Lines())
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

        private int CollectUniqueResults(string input)
            => ReadGroups(input)
                .Select(grp => grp.SelectMany(s => s).Distinct().Count())
                .Sum();
        
        private int CollectGroupResults(string input)
            => ReadGroups(input)
                .Select(grp => grp.SelectMany(s => s)
                    .GroupBy(c=>c)
                    .Where(g => g.Count() == grp.Count())
                    .Count()
                ).Sum();

        protected override long? Part1()
        {
            Assert(CollectUniqueResults(Sample()), 11);
            return CollectUniqueResults(Input);
        }

        protected override long? Part2()
        {
            Assert(CollectGroupResults(Sample()), 6);
            return CollectGroupResults(Input);
        }
    }
}
