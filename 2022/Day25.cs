namespace AdventOfCode._2022;

class Day25 : Solution<string>
{
    private const string RUNES = "=-012";
    private static long UnSNAFU(string number)
        => number.Reverse().Select((d, i) => (RUNES.IndexOf(d) - 2) * (long)Math.Pow(5, i)).Sum();

    private static string SNAFU(long number)
        => new(number.Unfold(n =>
        {
            var (d, r) = Math.DivRem(n + 2, 5);
            return (RUNES[(int)r], d);
        }, n => n != 0).Reverse().ToArray());

    private static string SumValues(string input)
        => SNAFU(input.Lines().Select(UnSNAFU).Sum());

    protected override string Part1()
    {
        Assert(UnSNAFU("2="), 8);
        Assert(UnSNAFU("20"), 10);
        Assert(UnSNAFU("2=-01"), 976);
        Assert(SNAFU(8), "2=");
        Assert(SNAFU(10), "20");
        Assert(SNAFU(976), "2=-01");
        Assert(SumValues(Sample()), "2=-1=0");
        return SumValues(Input);
    }
}
