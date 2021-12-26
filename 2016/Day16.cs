namespace AdventOfCode._2016;

using System;

class Day16 : Solution<string>
{
    private static string GenerateDataGetCheckSum(string input, int length)
    {
        char[] buffer = new char[length];
        Array.Copy(input.ToCharArray(), buffer, input.Length);

        int i = input.Length;
        while (i < length)
        {
            int aEnd = i - 1;
            buffer[i++] = '0';
            int stop = aEnd - Math.Min(length - 1 - i, aEnd);

            for (int bi = aEnd; bi >= stop; bi--)
                buffer[i++] = buffer[bi] == '0' ? '1' : '0';
        }

        while ((length & 1) == 0)
        {
            for (i = 0; i < length; i += 2)
                buffer[i >> 1] = (buffer[i] == buffer[i + 1]) ? '1' : '0';
            length >>= 1;
        }
        return new string(buffer, 0, length);
    }

    protected override string Part1()
    {
        Assert(GenerateDataGetCheckSum("10000", 20), "01100");
        return GenerateDataGetCheckSum(Input, 272);
    }

    protected override string Part2() => GenerateDataGetCheckSum(Input, 35_651_584);
}
