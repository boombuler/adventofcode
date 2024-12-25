namespace AdventOfCode._2024;

class Day25 : Solution
{
    private long Solve(string input)
    {
        var keysAndLocks = input.Split("\n\n").Select(StringExt.AsMap).ToLookup(
            m => m[(0, 0)], 
            m => m.Columns().Select(c => c.TakeWhile(x => x == m[(0, 0)]).Count()).ToArray());

        return keysAndLocks['.'].Sum(k =>
            keysAndLocks['#'].Count(l => k.Zip(l, (k, l) => l - k).All(d => d <= 0)));
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample()), 3);
        return Solve(Input);
    }
}
