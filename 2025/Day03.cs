namespace AdventOfCode._2025;

class Day03 : Solution
{
    private static IEnumerable<long> Solve(string input, int digits)
    {
        static long FindLargest(ReadOnlySpan<char> s, int digits)
        {
            if (digits == 0)
                return 0;

            int max = 0;
            int maxIndex = 0;
            
            for (int i = 0; i <= (s.Length - digits); i++)
            {
                if (s[i] - '0' is int cur && cur > max)
                {
                    max = cur;
                    maxIndex = i;
                }
            }
            long remaining = FindLargest(s.Slice(maxIndex + 1), digits - 1);
            return (max * (long)Math.Pow(10, digits - 1)) + remaining;
        }

        return input.Lines().Select(line => FindLargest(line, digits));
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample(), 2).SequenceEqual([98, 89, 78, 92]));
        return Solve(Input, 2).Sum();
    }

    protected override long? Part2()
    {
        Assert(Solve(Sample(), 12).SequenceEqual([
            987654321111,
            811111111119,
            434234234278,
            888911112111,
        ]));
        return Solve(Input, 12).Sum();
    }
}
