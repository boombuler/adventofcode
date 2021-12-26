namespace AdventOfCode._2015;

using System.Linq;
using AdventOfCode.Utils;

class Day08 : Solution
{
    private static long CountChars(string input)
        => input.Lines().Select(s => (long)s.Trim().Length).Sum();

    private static long CountUnquoted(string input)
    {
        long sum = 0;
        foreach (var l in input.Lines())
        {
            string line = l.Trim();
            sum += line.Length - 2;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\\')
                {
                    sum--;
                    if (line[++i] == 'x')
                    {
                        i += 2;
                        sum -= 2;
                    }
                }
            }
        }

        return sum;
    }

    private static long CountQuoted(string s)
    {
        long sum = 2;
        foreach (char c in s)
        {
            if (c is '"' or '\\')
                sum++;
            sum++;
        }
        return sum;
    }

    private static long CountQuotedStringChars(string input)
        => input.Lines().Select(x => CountQuoted(x.Trim())).Sum();

    protected override long? Part1()
    {
        Assert(CountChars(Sample()), 23);
        Assert(CountUnquoted(Sample()), 11);
        return CountChars(Input) - CountUnquoted(Input);
    }

    protected override long? Part2()
    {
        Assert(CountQuotedStringChars(Sample()), 42);
        return CountQuotedStringChars(Input) - CountChars(Input);
    }
}
