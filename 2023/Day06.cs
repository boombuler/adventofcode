namespace AdventOfCode._2023;

using static Parser;

class Day06 : Solution
{
    private static long RunRace(long time, long dist)
    {
        var d = Math.Sqrt((time * time) - (4 * dist));

        var z1 = (long)((time + d) / 2);
        var z2 = (long)((time - d) / 2);
        if (d == (long)d)
            z1--;
        return z1 - z2;
    }

    private static long Solve(string input, Parser<long> NumberParser)
    {
        Func<string, long[]> getNumbers = Any.Until(":").ThenR(NumberParser.Token().Many());
        var (times, (distances, _)) = input.Lines().Select(getNumbers);
        long total = 1;
        for (int race = 0; race < times.Length; race++)
            total *= RunRace(times[race], distances[race]);

        return total;
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample(), Int), 288);
        return Solve(Input, Int);
    }

    protected override long? Part2()
    {
        var parser = Digit.Token().Many1().Select(
            d => d.Aggregate(0L, MathExt.AppendDigit)
        );

        Assert(Solve(Sample(), parser), 71503);
        return Solve(Input, parser);
    }
}
