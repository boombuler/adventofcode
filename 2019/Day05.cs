namespace AdventOfCode._2019;

class Day05 : Solution
{
    private static long RunCode(string code, long input)
        => new IntCodeVM(code).Run([input]).Last();

    protected override long? Part1() => RunCode(Input, 1);

    protected override long? Part2()
    {
        Assert(RunCode("3,9,8,9,10,9,4,9,99,-1,8", 8), 1);
        Assert(RunCode("3,9,8,9,10,9,4,9,99,-1,8", 1), 0);
        Assert(RunCode("3,3,1107,-1,8,3,4,3,99", 1), 1);
        Assert(RunCode("3,3,1107,-1,8,3,4,3,99", 99), 0);
        return RunCode(Input, 5);
    }
}
