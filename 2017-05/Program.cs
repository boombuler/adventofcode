using System;
using System.Linq;
using AdventHelper;

namespace _2017_05
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private int CountJumps(string inputFile, int upperBound = int.MaxValue)
        {
            var lines = ReadLines(inputFile).Select(int.Parse).ToList();
            int PC = 0;
            int result = 0;

            while (PC >= 0 && PC < lines.Count)
            {
                var value = lines[PC];
                lines[PC] += (value >= upperBound) ? -1 : 1;
                PC += value;
                result++;
            }

            return result;
        }

        protected override long? Part1()
        {
            Assert(CountJumps("Sample"), 5);
            return CountJumps("Input");
        }

        protected override long? Part2()
        {
            Assert(CountJumps("Sample", 3), 10);
            return CountJumps("Input", 3);
        }

    }
}
