using System;
using System.Linq;

namespace AdventOfCode._2017
{
    class Day01 : Solution
    {
        private long SumDoubleDigits(string digits, int offset = 1)
        {
            long sum = 0;
            for (int i = 0; i < digits.Length; i++)
            {
                if (digits[i] == digits[(i + offset) % digits.Length])
                    sum += digits[i] - '0';
            }
            return sum;
        }

        protected override long? Part1() => SumDoubleDigits(Input);

        protected override long? Part2() => SumDoubleDigits(Input, Input.Length / 2);
    }
}
