namespace AdventOfCode._2025;

class Day03 : Solution
{
    private static IEnumerable<long> Solve(string input, int digits)
    {
        static long FindLargest(string s, int digits, Func<string, int, long> find)
        {
            if (digits == 1)
                return s.Max() - '0';
            else if (s.Length == digits)
                 return long.Parse(s);
            else
            {
                long a = (s[0]-'0')*((long)Math.Pow(10, digits-1)) + find(s[1..], digits - 1);
                long b = find(s[1..], digits);
                return a>b?a:b;
            }
        }
        var mem = Memoization.Recursive<string, int, long>(FindLargest);

        return input.Lines().Select(line => mem(line, digits));
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
