using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2021
{
    class Day03 : Solution
    {
        private bool IsCommonHi(List<string> Items, int Index)
        {
            int ones = Items.Count(itm => itm[Index] == '1');
            return ones >= Items.Count - ones;
        }

        private long SearchValue(bool CommonHi)
        {
            var items = Input.Lines().ToList();
            int idx = -1;
            while (items.Count > 1)
            {
                bool filter = IsCommonHi(items, ++idx) ^ (!CommonHi);
                items = items.Where(n => (n[idx] == '1') == filter).ToList();
            }
            return Convert.ToInt64(items.Single(), 2);
        }

        protected override long? Part1()
        {
            var input = Input.Lines().ToList();
            var bitLen = input.First().Length;
            long gamma = 0;
            long epsilon = 0;
            for (int i = 0; i < bitLen; i++)
            {
                if (IsCommonHi(input, bitLen - 1 - i))
                    gamma |= 1L << i;
                else
                    epsilon |= 1L << i;
            }
            return gamma * epsilon;
        }

        protected override long? Part2()
            => SearchValue(CommonHi: true) * SearchValue(CommonHi: false);
    }
}
