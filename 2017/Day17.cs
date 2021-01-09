using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2017
{
    class Day17 : Solution
    {
        private int GetValueAfterLastInsert(int steps)
        {
            var buffer = new List<int>(2017) { 0 };
            var current = 0;
            for (int i = 1; i <= 2017; i++)
                buffer.Insert(current = 1 + (current + steps) % i, i);
            return buffer[(current + 1) % buffer.Count];
        }

        protected override long? Part1()
        {
            Assert(GetValueAfterLastInsert(3), 638);
            return GetValueAfterLastInsert(int.Parse(Input));
        }

        protected override long? Part2()
        {
            long result = 0;
            int current = 0;
            int steps = int.Parse(Input);
            for (int i = 1; i <= 50_000_000; i++)
            {
                current = 1 + (current + steps) % i;
                if (current == 1)
                    result = i;
            }
            return result;
        }
    }
}
