namespace AdventOfCode._2023;

class Day09 : Solution
{
    private static (int First, int Last) SumNextValues(string input)
        => input.Lines().Select(l => l.Split(' ').Select(int.Parse).ToArray())
            .Select(data => data.Unfold(d => d.SlidingWindow(2).Select(n => n[1] - n[0]).ToArray())
                .Prepend(data)
                .TakeWhile(d => d.Any(x => x != 0))
                .Aggregate((f: 0, l: 0, s: 1), (a, d) => (a.f + (a.s * d[0]), a.l + d[^1], -a.s)))
            .Aggregate((First: 0, Last: 0), (a, d) => (a.First + d.f, a.Last + d.l));

    protected override long? Part1()
    {
        Assert(SumNextValues(Sample()).Last, 114);
        return SumNextValues(Input).Last;
    }

    protected override long? Part2()
    {
        Assert(SumNextValues(Sample()).First, 2);
        return SumNextValues(Input).First;
    }
}
