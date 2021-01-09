using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2016
{
    class Day06 : Solution<string>
    {
        private char MostCommon(IEnumerable<char> chars)
            => chars.GroupBy(c => c).OrderByDescending(g => g.Count()).First().Key;

        private char LeastCommon(IEnumerable<char> chars)
            => chars.GroupBy(c => c).OrderBy(g => g.Count()).First().Key;

        private string DecodeMessage(string data, Func<IEnumerable<char>, char> charSelector)
            => new string(
               data.Lines()
                    .SelectMany(line => line.Select((chr, idx) => new { chr, idx })) // each char in the file + index in line
                    .GroupBy(x => x.idx) // grouped by char index
                        .Select(grp => new {
                            idx = grp.Key,
                            chr = charSelector(grp.Select(e => e.chr))
                        })
                .OrderBy(x => x.idx).Select(x => x.chr).ToArray() // Convert to ordered char array.
            );


        protected override string Part1()
        {
            Func<string, string> decode = (file) => DecodeMessage(file, MostCommon);
            Assert(decode(Sample()), "easter");
            return decode(Input);
        }

        protected override string Part2()
        {
            Func<string, string> decode = (file) => DecodeMessage(file, LeastCommon);
            Assert(decode(Sample()), "advent");
            return decode(Input);
        }
    }
}
