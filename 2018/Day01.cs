namespace AdventOfCode._2018;

using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day01 : Solution
{
    protected override long? Part1() => Input.Lines().Select(long.Parse).Sum();

    protected override long? Part2()
    {
        var cur = 0L;
        var seen = new HashSet<long>() { 0 };
        var changes = Input.Lines().Select(long.Parse).ToArray();

        for (int i = 0; ; i++)
        {
            cur += changes[i % changes.Length];
            if (!seen.Add(cur))
                return cur;
        }
    }
}
