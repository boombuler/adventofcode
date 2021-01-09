using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode._2017
{
    class Day06 : Solution
    {
        private (int LoopAfter, int LoopSize) FindDistributionLoop(params int[] banks)
        {
            var seen = new Dictionary<BigInteger, int>();
            var shift = (int)Math.Ceiling(Math.Log2(banks.Sum())); // how many bits are needed to represent the sum of all memory blocks?

            BigInteger ToBigInt()
            {
                var res = new BigInteger(0);
                for (int i = 0; i < banks.Length; i++)
                    res = (res << shift) + banks[i];
                return res;
            }

            var loopAfter = 0;
            while(seen.TryAdd(ToBigInt(), loopAfter))
            {
                int max = 0;
                for(int i = 0; i < banks.Length; i++)
                {
                    if (banks[i] > banks[max])
                        max = i;
                }

                var value = banks[max];
                banks[max] = 0;

                for (int i = 0; i < banks.Length; i++)
                    banks[i] += value / banks.Length;
                for (int i = 1; i <= value % banks.Length; i++)
                    banks[(max + i) % banks.Length]++;

                loopAfter++;
            }
            var loopSize = loopAfter - seen[ToBigInt()];
            return (loopAfter, loopSize);
        }

        private int[] Banks => Input.Split('\t').Select(int.Parse).ToArray();

        protected override long? Part1()
        {
            Assert(FindDistributionLoop(0, 2, 7, 0).LoopAfter, 5);
            return FindDistributionLoop(Banks).LoopAfter;
        }

        protected override long? Part2()
        {
            Assert(FindDistributionLoop(0, 2, 7, 0).LoopSize, 4);
            return FindDistributionLoop(Banks).LoopSize;
        }
    }
}
