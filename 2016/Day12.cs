namespace AdventOfCode._2016;

class Day12 : Solution
{
    private static long Run(string code, long cInit = 0)
        => new AssembunnyVM(code).Run(c: cInit);

    protected override long? Part1()
    {
        Assert(Run(Sample()), 42);
        return Run(Input);
    }

    protected override long? Part2() => Run(Input, 1);
}
