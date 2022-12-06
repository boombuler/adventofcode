namespace AdventOfCode._2019;

class Day16 : Solution<string>
{
    private static int Factor(int digit, int idx)
    {
        idx++;
        idx /= digit + 1;
        return new[] { 0, 1, 0, -1 }[idx % 4];
    }

    private static void FFTRound(IEnumerable<char> input, char[] output)
    {
        for (int i = 0; i < output.Length; i++)
        {
            var sum = input.Select((digit, idx) => (digit - '0') * Factor(i, idx)).Sum();
            output[i] = (char)('0' + (Math.Abs(sum) % 10));
        }
    }

    private static string FTT(string input, int rounds, int digits = 8)
    {
        char[] inchars = input.ToArray();
        char[] outchars = new char[inchars.Length];

        for (int round = 0; round < rounds; round++)
        {
            FFTRound(inchars, outchars);
            (inchars, outchars) = (outchars, inchars);
        }

        return new string(inchars.Take(digits).ToArray());
    }

    protected override string Part1()
    {
        Assert(FTT("12345678", 1), "48226158");
        Assert(FTT("12345678", 2), "34040438");
        Assert(FTT("12345678", 3), "03415518");
        Assert(FTT("12345678", 4), "01029498");
        Assert(FTT("80871224585914546619083218645595", 100), "24176176");
        Assert(FTT("19617804207202209144916044189917", 100), "73745418");
        Assert(FTT("69317163492948606335995924319873", 100), "52432133");

        return FTT(Input, 100);
    }

    protected override string Part2()
    {
        int offset = int.Parse(new string(Input.Take(7).ToArray()));
        int skip = (offset / Input.Length);
        offset -= skip * Input.Length;

        var buffer = new char[((10_000 - skip) * Input.Length) - offset];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = Input[(i + offset) % Input.Length];

        for (int r = 0; r < 100; r++)
        {
            int sum = 0;
            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                sum += buffer[i] - '0';
                buffer[i] = (char)('0' + (Math.Abs(sum) % 10));
            }
        }

        return new string(buffer.Take(8).ToArray());
    }

}
