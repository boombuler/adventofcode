namespace AdventOfCode._2020;

using System.Collections.Generic;
using System.Linq;

class Day15 : Solution
{
    private static int CountNumbers(int turns, params int[] input)
    {
        var mem = new Dictionary<int, int>(turns / 2);

        for (int i = 0; i < input.Length - 1; i++)
            mem[input[i]] = i;
        int lastNum = input.Last();
        for (int i = input.Length - 1; i < turns - 1; i++)
        {
            int newNum = mem.TryGetValue(lastNum, out int t) ? i - t : 0;
            mem[lastNum] = i;
            lastNum = newNum;
        }
        return lastNum;
    }

    private int[] InputNumbers => Input.Split(',').Select(int.Parse).ToArray();
    protected override long? Part1()
    {
        Assert(CountNumbers(2020, 0, 3, 6), 436);
        return CountNumbers(2020, InputNumbers);
    }

    protected override long? Part2()
    {
        Assert(CountNumbers(30_000_000, 0, 3, 6), 175_594);
        return CountNumbers(30_000_000, InputNumbers);
    }
}
