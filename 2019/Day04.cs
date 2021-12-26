namespace AdventOfCode._2019;

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day04 : Solution
{
    private IEnumerable<IEnumerable<int>> GenDigits(int startingDigit, int count)
    {
        for (int i = startingDigit; i <= 9; i++)
        {
            if (count == 1)
                yield return new[] { i };
            else
            {
                foreach (var p in GenDigits(i, count - 1))
                    yield return p.Prepend(i);
            }
        }
    }

    private static IEnumerable<int> DigitGroupSizes(IEnumerable<int> digits)
    {
        var (curDigit, other) = digits;
        var count = 1;
        foreach (var d in other)
        {
            if (d == curDigit)
                count++;
            else
            {
                yield return count;
                (curDigit, count) = (d, 1);
            }
        }
        yield return count;
    }

    private IEnumerable<int> GenPasswordCandidates(Func<int, bool> groupRule)
        => GenDigits(0, 6).Where(num => DigitGroupSizes(num).Any(groupRule))
            .Select(num => num.Aggregate(0, (sum, d) => (sum * 10) + d));

    public int CountPasswordsWithGroupRule(Func<int, bool> groupRule)
    {
        var (lo, (hi, _)) = Input.Split('-').Select(int.Parse);
        return GenPasswordCandidates(groupRule).Where(n => n >= lo && n <= hi).Count();
    }

    protected override long? Part1() => CountPasswordsWithGroupRule(d => d >= 2);
    protected override long? Part2() => CountPasswordsWithGroupRule(d => d == 2);
}
