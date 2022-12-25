namespace AdventOfCode._2022;

class Day25 : Solution<string>
{
    private readonly Dictionary<(char Rune, int Digit), (long Min, long Max, long Value)> fDigitLookup = new();
    private const string RUNES = "210-=";

    private (long Min, long Max, long Value) GetDigitValues(char Rune, int Digit)
    {
        if (Digit < 0)
            return (0, 0, 0);
        var key = (Rune, Digit);
        if (!fDigitLookup.ContainsKey(key))
        {
            var pow = (long)Math.Pow(5, Digit);
            var (pMax, pMin) = (GetDigitValues('2', Digit - 1).Max, GetDigitValues('=', Digit - 1).Min);
            fDigitLookup[('2', Digit)] = ((pow * +2 + pMin), (pow * +2 + pMax), (pow * +2));
            fDigitLookup[('1', Digit)] = ((pow * +1 + pMin), (pow * +1 + pMax), (pow * +1));
            fDigitLookup[('0', Digit)] = ((pow * +0 + pMin), (pow * +0 + pMax), (pow * +0));
            fDigitLookup[('-', Digit)] = ((pow * -1 + pMin), (pow * -1 + pMax), (pow * -1));
            fDigitLookup[('=', Digit)] = ((pow * -2 + pMin), (pow * -2 + pMax), (pow * -2));
        }
        return fDigitLookup[key];
    }

    private long UnSNAFU(string number)
        => number.Reverse().Select((d, i) => GetDigitValues(d, i).Value).Sum();

    private string SNAFU(long number)
    {
        int digits = 0.Unfold(n => n + 1).First(d => GetDigitValues(RUNES.First(), d).Max > number);

        long remaining = number;
        var result = new StringBuilder();
        for (int d = digits; d >= 0; d--)
        {
            foreach(var r in RUNES)
            {
                var (min, _, value) = GetDigitValues(r, d);
                if (remaining >= min)
                {
                    result.Append(r);
                    remaining -= value;
                    break;
                }
            }
        }

        return result.ToString();
    }

    private string SumValues(string input)
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
