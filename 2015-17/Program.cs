using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2015_17
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private IEnumerable<int> GetPossibilities(string input, long liters)
        {
            IEnumerable<int> count(IEnumerable<long> containers, long rest, int usedItems)
            {
                if (!containers.Any())
                    return Enumerable.Empty<int>();
                var i = containers.First();
                var others = containers.Skip(1);
                var rem = rest - i;
                var result = count(others, rest, usedItems).ToList();
                if (rem == 0)
                {
                    result.Add(usedItems + 1);
                    return result;
                }
                if (rem < 0)
                    return result;
                result.AddRange(count(others, rem, usedItems + 1));
                return result;
            }

            var items = ReadLines(input).Select(long.Parse).ToList();
            return count(items, liters, 0).Where(x => x > 0);
        }

        private long FindMinimumPossibilities(string input, long liters)
        {
            var pos = GetPossibilities(input, liters).ToList();
            var minComb = pos.Min();
            return pos.Where(x => x == minComb).Count();
        }

        protected override long? Part1()
        {
            Assert(GetPossibilities("Sample", 25).Count(), 4);
            return GetPossibilities("Input", 150).Count();
        }

        protected override long? Part2()
        {
            Assert(FindMinimumPossibilities("Sample", 25), 3);
            return FindMinimumPossibilities("Input", 150);
        }
    }
}
