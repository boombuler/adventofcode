using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2020_09
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private bool IsValidNumber(IEnumerable<long> preamble, long number)
        {
            for (int iA = 0; iA < preamble.Count() - 1; iA++)
            {
                var items = preamble.Skip(iA);
                long a = items.First();
                foreach (long b in items.Skip(1))
                {
                    if (b == a)
                        continue;
                    if (a + b == number)
                        return true;
                }
            }
            return false;
        }

        private long FindFirstInvalidNumber(string inputFile, int preambleSize)
        {
            var preamble = new LinkedList<long>();
            foreach(var line in ReadLines(inputFile))
            {
                long value = long.Parse(line);
                if (preamble.Count == preambleSize && !IsValidNumber(preamble, value))
                    return value;
                preamble.AddFirst(value);
                if (preamble.Count > preambleSize)
                    preamble.RemoveLast();
            }
            throw new Exception("Only valid data");
        }

        private long FindWeakness(string inputFile, int preambleSize)
        {
            var target = FindFirstInvalidNumber(inputFile, preambleSize);
            var numbers = ReadLines(inputFile).Select(long.Parse).ToList();
            for (int start = 0; start < numbers.Count-1; start++)
            {
                long sum = numbers[start];
                for (int end = start+1; end < numbers.Count; end++)
                {
                    sum += numbers[end];
                    if (sum == target)
                    {
                        var items = numbers.Skip(start).Take(end - start);
                        return items.Min() + items.Max();
                    }
                    if (sum > target)
                        break;
                }
            }
            throw new Exception("Only valid data");
        }

        protected override long? Part1()
        {
            Assert(FindFirstInvalidNumber("SampleInput", 5), 127);
            return FindFirstInvalidNumber("Input", 25);
        }
        protected override long? Part2()
        {
            Assert(FindWeakness("SampleInput", 5), 62);
            return FindWeakness("Input", 25);
        }
    }
}
