﻿namespace AdventOfCode._2016;

using System.Numerics;

class Day15 : Solution
{
    record Disc(int No, int Positions, int Offset);
    private static readonly Func<string, Disc> ParseDisc = new Regex(@"Disc #(?<No>\d+) has (?<Positions>\d+) positions; at time=0, it is at position (?<Offset>\d+)\.", RegexOptions.Compiled)
        .ToFactory<Disc>();

    public static long GetButtonPressTime(string discDescriptions)
    {
        var discs = discDescriptions.Lines().Select(ParseDisc);
        return (long)discs
            .Select(d => (a: -(BigInteger)d.Offset - d.No, n: (BigInteger)d.Positions))
            .Aggregate(MathExt.ChineseRemainder).a;
    }

    protected override long? Part1()
    {
        Assert(GetButtonPressTime(Sample()), 5);
        return GetButtonPressTime(Input);
    }

    protected override long? Part2() => GetButtonPressTime(Input + "\nDisc #7 has 11 positions; at time=0, it is at position 0.");
}
