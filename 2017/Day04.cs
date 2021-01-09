using AdventOfCode.Utils;
using System;
using System.Linq;

namespace AdventOfCode._2017
{
    class Day04 : Solution
    {
        protected override long? Part1()
            => Input.Lines().Select(l => l.Split(' '))
                .Where(words => words.Length == words.Distinct().Count())
                .Count();

        protected override long? Part2()
            => Input.Lines()
                .Select(l => l.Split(' ')
                    .Select(w => new string(w.OrderBy(c => c).ToArray()))
                    .ToArray()
                )
                .Where(words => words.Length == words.Distinct().Count())
                .Count();
    }
}
