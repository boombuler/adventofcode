using AdventHelper;
using System;
using System.Linq;

namespace _2017_02
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private long CalcChecksum(string input)
            => ReadLines(input).Select(l =>
            {
                var numbers = l.Split('\t', ' ').Select(int.Parse);
                return numbers.Max() - numbers.Min();
            }).Sum();

        protected override long? Part1()
        {
            Assert(CalcChecksum("Sample"), 18);
            return CalcChecksum("Input");
        }


        private long SumEvenlyDivisibleValues(string input)
        {
            var sum = 0L;
            foreach (var line in ReadLines(input).Select(l => l.Split('\t', ' ').Select(int.Parse).ToArray()))
            { 
                foreach(var nominator in line)
                {
                    var denominator = line.Where(d => d != nominator && nominator % d == 0).FirstOrDefault();
                    if (denominator > 0)
                    {
                        sum += (nominator / denominator);
                        break;
                    }
                }
            }
            return sum;
        }

        protected override long? Part2()
        {
            Assert(SumEvenlyDivisibleValues("Sample2"), 9);
            return SumEvenlyDivisibleValues("Input");
        }
    }
}
