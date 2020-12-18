using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2020_15
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private int CountNumbers(int turns, params int[] input)
        {
            var mem = new Dictionary<int, int>(turns/2);

            for (int i = 0; i < input.Length-1; i++)
                mem[input[i]] = i;
            int lastNum = input.Last();
            for (int i = input.Length-1; i < turns-1; i++)
            {
                int newNum = mem.TryGetValue(lastNum, out int t) ? i-t : 0;
                mem[lastNum] = i;
                lastNum = newNum;
            }
            return lastNum;
        }

        protected override long? Part1()
        {
            Assert(CountNumbers(2020, 0, 3, 6), 436);
            return CountNumbers(2020, 9, 6, 0, 10, 18, 2, 1);
        }

        protected override long? Part2()
        {
            Assert(CountNumbers(30_000_000, 0, 3, 6), 175_594);
            return CountNumbers(30_000_000, 9, 6, 0, 10, 18, 2, 1);
        }
    }
}
