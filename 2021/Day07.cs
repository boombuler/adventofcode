namespace AdventOfCode._2021;

using System;
using System.Linq;
using AdventOfCode.Utils;

class Day07 : Solution
{
    private long Solve(Func<int, long> GetFuel)
    {
        var positions = Input.Split(',').Select(int.Parse).ToList();
        var (min, max) = positions.MinMax();
        return Enumerable.Range(min, max - min).Select(d => positions.Sum(p => GetFuel(Math.Abs(p - d)))).Min();
    }
    protected override long? Part1() => Solve(m => m);
    protected override long? Part2() => Solve(m => (m * (m + 1)) / 2);
}
