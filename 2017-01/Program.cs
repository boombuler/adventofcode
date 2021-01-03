using AdventHelper;
using System;
using System.Linq;

namespace _2017_01
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

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

        protected override long? Part1() => SumDoubleDigits(ReadLines("Input").Single());

        protected override long? Part2()
        {
            string digits = ReadLines("Input").Single();
            return SumDoubleDigits(digits, digits.Length / 2);
        }
    }
}
