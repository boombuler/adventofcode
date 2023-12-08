namespace AdventOfCode._2023;

using static Parser;

class Day06 : Solution
{
    private static long RunRace(long time, long distance)
    {
        var d = Math.Sqrt(time * time - 4 * (distance + 1));
        var z = (long)Math.Ceiling((time - d) / 2);
        return time - 2*z + 1;
    }

    private static long Solve(string input, Parser<long> NumberParser)
    {
        Func<string, long[]> getNumbers = Any.Until(":").ThenR(NumberParser.Token().Many());
        var (times, (distances, _)) = input.Lines().Select(getNumbers);
        return times.Zip(distances, RunRace).Aggregate((a, b) => a * b);
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
