using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2016
{
    class Day03 : Solution
    {
        private bool TriangleValid(long A, long B, long C) => (A + B) > C && (A + C) > B && (B + C) > A;

        private IEnumerable<long> Numbers()
            => Input.Lines().SelectMany(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries)).Select(long.Parse);

        private IEnumerable<long> Transpose(IEnumerable<long> nums)
            => nums.Select((n, i) => new { n, i }).GroupBy(e => e.i % 3).SelectMany(grp => grp).Select(e => e.n);

        private long CountValid(IEnumerable<long> nums)
            => nums.Select((n, i) => new { n, i })
                .GroupBy(e => e.i / 3)
                .Select(grp => grp.Select(e => e.n).ToArray())
                .Where(g => TriangleValid(g[0], g[1], g[2]))
                .Count();

        protected override long? Part1()
        {
            Assert(!TriangleValid(5, 10, 25));
            return CountValid(Numbers());
        }

        protected override long? Part2() => CountValid(Transpose(Numbers()));
    }
}
