using AdventHelper;
using System;
using System.Linq;

namespace _2020_01
{
    class Program : ProgramBase
    {
        private static int? Solve(int[] items, int factorCount, int targetSum = 2020)
        {
            if (factorCount < 2 || factorCount > items.Length)
                throw new ArgumentException("Invalid Factor Count");

            int? TrySolve(int sum, int factors, int startIdx)
            {
                for (int i = startIdx; i < items.Length; i++)
                {
                    var value = items[i];
                    if (factors == 1)
                    {
                        if (value + sum == targetSum)
                            return value;
                    }
                    else
                    {
                        var result = TrySolve(sum + value, factors - 1, i + 1);
                        if (result.HasValue)
                            return result * value;
                    }
                }
                return null;
            }
            return TrySolve(0, factorCount, 0);
        }

        protected override long? Part1() => Solve(ReadLines().Select(int.Parse).ToArray(), 2);
        protected override long? Part2() => Solve(ReadLines().Select(int.Parse).ToArray(), 3);
       
        static void Main(string[] args) => new Program().Run();
    }
}
