using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2019
{
    class Day09 : Solution
    {
        protected override long? Part1()
            => new IntCodeVM(Input).Run(new long[] { 1 }).First();

        protected override long? Part2()
            => new IntCodeVM(Input).Run(new long[] { 2 }).First();
    }
}
