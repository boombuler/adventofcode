namespace AdventOfCode._2016;

class Day23 : Solution
{
    protected override long? Part1()
    {
        Assert(new AssembunnyVM(Sample()).Run(), 3);
        return new AssembunnyVM(Input).Run(7);
    }

    protected override long? Part2() => new AssembunnyVM(Input).Run(12);
}
