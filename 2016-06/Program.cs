using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2016_06
{
    class Program : ProgramBase<string>
    {
        static void Main(string[] args) => new Program().Run();

        private char MostCommon(IEnumerable<char> chars)
            => chars.GroupBy(c => c).OrderByDescending(g => g.Count()).First().Key;

        private char LeastCommon(IEnumerable<char> chars)
            => chars.GroupBy(c => c).OrderBy(g => g.Count()).First().Key;

        private string DecodeMessage(string file, Func<IEnumerable<char>, char> charSelector)
            => new string(
                ReadLines(file)
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
            Assert(decode("Sample.txt"), "easter");
            return decode("Input.txt");
        }

        protected override string Part2()
        {
            Func<string, string> decode = (file) => DecodeMessage(file, LeastCommon);
            Assert(decode("Sample.txt"), "advent");
            return decode("Input.txt");
        }
    }
}
