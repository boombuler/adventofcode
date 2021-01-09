using AdventOfCode.Utils;
using System;
using System.Linq;

namespace AdventOfCode._2017
{
    class Day05 : Solution
    {
        private int CountJumps(string input, int upperBound = int.MaxValue)
        {
            var lines = input.Lines().Select(int.Parse).ToList();
            int PC = 0;
            int result = 0;

            while (PC >= 0 && PC < lines.Count)
            {
                var value = lines[PC];
                lines[PC] += (value >= upperBound) ? -1 : 1;
                PC += value;
                result++;
            }

            return result;
        }

        protected override long? Part1()
        {
            Assert(CountJumps(Sample()), 5);
            return CountJumps(Input);
        }

        protected override long? Part2()
        {
            Assert(CountJumps(Sample(), 3), 10);
            return CountJumps(Input, 3);
        }

    }
}
