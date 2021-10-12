﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2019
{
    class Day02 : Solution
    {
        private const int OC_HALT = 99;
        private const int OC_ADD = 1;
        private const int OC_MUL = 2;
        private long RunIntCode(string code, long? noun = null, long? verb = null)
        {
            var vm = new IntCodeVM(code);
            if (noun.HasValue)
                vm[1] = noun.Value;
            if (verb.HasValue)
                vm[2] = verb.Value;

            try
            {
                foreach (var _ in vm.Run()) ;

                return vm[0];
            }
            catch(Exception e)
            {
                Error(e.Message);
                return 0;
            }
        }

        protected override long? Part1()
        {
            Assert(RunIntCode("1,0,0,0,99"), 2);
            Assert(RunIntCode("1,1,1,4,99,5,6,0,99"), 30);
            return RunIntCode(Input, 12, 2);
        }

        protected override long? Part2()
        {
            for (int n = 0; n < 100; n++)
                for (int v = 0; v < 100; v++)
                {
                    var res = RunIntCode(Input, n, v);
                    if (res == 19690720)
                        return (n * 100) + v;
                }
            return null;
        }
    }
}