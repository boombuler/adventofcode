namespace AdventOfCode._2025;

class Day03 : Solution
{
    private static IEnumerable<long> Solve(string input, int digits)
        => input.Lines().Select(s => 
            Enumerable.Range(1, digits).Reverse()
                .Aggregate((sum: 0L, txt: s), (seed, d) =>
                {
                    var (max, idx) = seed.txt[..^(d - 1)]
                        .Select((chr, idx) => (chr, idx))
                        .MaxBy(x => x.chr);
                    return (seed.sum * 10 + max - '0', seed.txt[(idx + 1)..]);
                }).sum);

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
