namespace AdventOfCode._2025;

using static Parser;

class Day02 : Solution
{
    private IEnumerable<long> Iterate(string input, string pattern)
    {
        var check = new Regex(pattern, RegexOptions.Compiled);
        var ranges = Long.ThenL(Char('-')).Then(Long).List(',').MustParse(input);
        
        foreach (var (min, max) in ranges)
        {
            for (long n = min; n <= max; n++)
            {
                if (check.IsMatch(n.ToString()))
                    yield return n;
            }
        }
    }

    protected override long? Part1()
    {
        const string pattern = @"^(\d+)\1$";
        Assert(Iterate(Sample(), pattern).SequenceEqual([
            11, 22, 99, 1010, 1188511885, 222222, 446446, 38593859
        ]));
        return Iterate(Input, pattern).Sum();
    }

    protected override long? Part2()
    {
        const string pattern = @"^(\d+)\1+$";
        Assert(Iterate(Sample(),pattern).SequenceEqual([
            11, 22, 99, 111, 999, 1010, 1188511885, 222222, 446446, 38593859, 565656, 824824824, 2121212121
        ]));
        return Iterate(Input, pattern).Sum();
    }
}
