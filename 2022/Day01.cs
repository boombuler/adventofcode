namespace AdventOfCode._2022;

using System.Linq;
using AdventOfCode.Utils;

class Day01 : Solution
{
    private long GetCalorieSum(int elfCount)
        => Input.Split("\n\n")
            .Select(l => l.Lines().Select(long.Parse).Sum())
            .OrderByDescending(d => d)
            .Take(elfCount)
            .Sum();

    protected override long? Part1() => GetCalorieSum(1);
    protected override long? Part2() => GetCalorieSum(3);
}
