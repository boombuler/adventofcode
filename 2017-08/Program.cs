﻿using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2017_08
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private (long MaxValue, Dictionary<string, long> Registers) ExecuteInstructions(string inputFile)
        {
            var result = new Dictionary<string, long>();
            long maxValue = 0;
            foreach(var line in ReadLines(inputFile))
            {
                // 0  1   2   3  4  5  6
                // c inc -20 if  c == 10
                var parts = line.Split(' ');

                if (!result.TryGetValue(parts[4], out long cmpA))
                    cmpA = 0L;
                var cmpB = long.Parse(parts[6]);
                bool shouldAdd = parts[5] switch
                {
                    ">"  => cmpA > cmpB,
                    "<"  => cmpA < cmpB,
                    ">=" => cmpA >= cmpB,
                    "<=" => cmpA <= cmpB,
                    "==" => cmpA == cmpB,
                    _    => cmpA != cmpB
                };
                if (shouldAdd)
                {
                    var value = long.Parse(parts[2]);
                    if (parts[1] == "dec")
                        value = -value;

                    var destReg = parts[0];
                    if (result.TryGetValue(destReg, out long oldVal))
                        value += oldVal;
                    
                    maxValue = Math.Max(maxValue, value);
                    result[destReg] = value;
                }
            }
            return (maxValue, result);
        }

        private long LargestRegisterValue(string inputFile)
            => ExecuteInstructions(inputFile).Registers.Values.Max();

        protected override long? Part1()
        {
            Assert(LargestRegisterValue("Sample"), 1);
            return LargestRegisterValue("Input");
        }

        protected override long? Part2()
        {
            Assert(ExecuteInstructions("Sample").MaxValue, 10);
            return ExecuteInstructions("Input").MaxValue;
        }
    }
}
