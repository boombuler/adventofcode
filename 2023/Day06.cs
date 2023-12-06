namespace AdventOfCode._2023;

using System.Numerics;
using static Parser;

class Day06 : Solution
{
    private static long RunRace(BigInteger time, BigInteger dist)
    {
        bool WouldWin(BigInteger t) => 
            ((time - t) * t) > dist;

        BigInteger left = (time / 2).Unfold(i => i-1).TakeWhile(WouldWin).Last();
        BigInteger right = (time / 2).Unfold(i => i+1).TakeWhile(WouldWin).Last();
        return (long)(right - left)+1;
    }

    private static long Solve(string input, Parser<BigInteger> NumberParser)
    {
        Func<string, BigInteger[]> getNumbers = Any.Until(":").ThenR(NumberParser.Many1());
        var (times, (distances, _)) = input.Lines().Select(getNumbers);
        long total = 1;
        for (int race = 0; race < times.Length; race++)
            total *= RunRace(times[race], distances[race]);

        return total;
    }

    protected override long? Part1()
    {
        var parser = BigInt.Token();
        Assert(Solve(Sample(), parser), 288);
        return Solve(Input, parser);
    }

    protected override long? Part2()
    {
        var parser = Digit.Token().Many1().Select(
            d => d.Select(n => new BigInteger(n - '0')).Aggregate((a, b) => (a * 10) + b)
        );
        Assert(Solve(Sample(), parser), 71503);
        return Solve(Input, parser);
    }
}
