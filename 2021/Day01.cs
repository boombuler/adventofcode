using AdventOfCode.Utils;
using System.Linq;

namespace AdventOfCode._2021
{
    class Day01 : Solution
    {
        private long Solve(int windowSize) 
            => Input.Lines().Select(long.Parse)
                .SlidingWindow(windowSize).Select(Enumerable.Sum)
                .SlidingWindow(2).Count(n => n[1] > n[0]);

        protected override long? Part1() => Solve(1);

        protected override long? Part2() => Solve(3);    
    }
}
