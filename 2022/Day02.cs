namespace AdventOfCode._2022;

using System;
using System.Linq;
using AdventOfCode.Utils;

class Day02 : Solution
{
    const int LOSS_SCORE = 0;
    const int DRAW_SCORE = 3;
    const int WON_SCORE = 6;

    private int Solve(Func<int, int, int> scoreCalc)
        => Input.Lines().Sum(s => scoreCalc(s[0] - 'A', s[2] - 'X'));

    protected override long? Part1()
    {
        var scores = new[] { DRAW_SCORE, LOSS_SCORE, WON_SCORE };
        return Solve((a, x) => x + 1 + scores[(a + 3 - x) % 3]);
    }

    protected override long? Part2()
    {
        var offsets = new[] { 2, 0, 1 };
        var scores = new[] { LOSS_SCORE, DRAW_SCORE, WON_SCORE };

        return Solve((a,x) => scores[x] + 1 + ((a + offsets[x]) % 3));
    }
}
