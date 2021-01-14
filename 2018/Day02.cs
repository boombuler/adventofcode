using AdventOfCode.Utils;
using System.Linq;

namespace AdventOfCode._2018
{
    class Day02 : Solution<long?, string>
    {
        protected override long? Part1()
        {
            var charCounts = Input.Lines().Select(l => l.GroupBy(c => c).Select(g => g.Count()).ToHashSet()).ToList();
            return charCounts.Count(chrs => chrs.Contains(2)) * charCounts.Count(chrs => chrs.Contains(3));
        }

        protected override string Part2()
            => Input.Lines().Combinations(2)
                .Select(itm => string.Concat(itm.First().Zip(itm.Last(), (ac, bc) => ac == bc ? ac.ToString() : string.Empty)))
                .OrderByDescending(s => s.Length)
                .First();
    }
}
