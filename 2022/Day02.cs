namespace AdventOfCode._2022;

using System;
using System.Linq;
using AdventOfCode.Utils;

class Day02 : Solution
{
    const int LossScore = 0;
    const int DrawScore = 3;
    const int WonScore = 6;

    private int Solve(Func<int, int, int> scoreCalc)
        => Input.Lines().Sum(s => scoreCalc(s[0] - 'A', s[2] - 'X'));

    protected override long? Part1() => Solve(
        (a, x) => x + 1 + ((a + 3 - x) % 3) switch
        {
            0 => DrawScore,
            1 => LossScore,
            _ => WonScore
        });

    protected override long? Part2()
    {
        var offsets = new[] { 2, 0, 1 };
        var scores = new[] { LossScore, DrawScore, WonScore };

        return Solve((a,x) => scores[x] + 1 + ((a + offsets[x]) % 3));
    }
}
