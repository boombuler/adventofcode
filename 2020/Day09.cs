using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2020
{
    class Day09 : Solution
    {
        private bool IsValidNumber(IEnumerable<long> preamble, long number)
        => preamble.Where((a, iA) => preamble.Skip(iA).Where(b => b != a).Any(b => b + a == number)).Any();

        private long FindFirstInvalidNumber(string input, int preambleSize)
        {
            var preamble = new LinkedList<long>();
            foreach(var line in input.Lines())
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

        private long FindWeakness(string input, int preambleSize)
        {
            var target = FindFirstInvalidNumber(input, preambleSize);
            var numbers = input.Lines().Select(long.Parse).ToList();
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
            Assert(FindFirstInvalidNumber(Sample(), 5), 127);
            return FindFirstInvalidNumber(Input, 25);
        }
        protected override long? Part2()
        {
            Assert(FindWeakness(Sample(), 5), 62);
            return FindWeakness(Input, 25);
        }
    }
}
