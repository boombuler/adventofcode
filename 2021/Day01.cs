using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021
{
    class Day01 : Solution
    {
        protected override long? Part1()
            => Input.Lines().Select(long.Parse)
                .SlidingWindow(2).Count(n => n[1] > n[0]);

        protected override long? Part2()
            => Input.Lines().Select(long.Parse)
                .SlidingWindow(3).Select(Enumerable.Sum)
                .SlidingWindow(2).Count(n => n[1] > n[0]);
    }
}
