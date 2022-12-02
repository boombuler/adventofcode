namespace AdventOfCode._2022;

using System.Diagnostics;
using System.Linq;
using AdventOfCode.Utils;

class Day01 : Solution
{
    private long GetCalorieSum(int elfCount)
    {
        var maxGroups = new MaxHeap<long>(
            Input.Split("\n\n").Select(l => l.Lines().Sum(long.Parse))
        );
        return Enumerable.Range(0, elfCount).Sum(_ => maxGroups.Pop());
    }

    protected override long? Part1() => GetCalorieSum(1);
    protected override long? Part2() => GetCalorieSum(3);
}
