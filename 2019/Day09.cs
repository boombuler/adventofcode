namespace AdventOfCode._2019;

class Day09 : Solution
{
    protected override long? Part1()
        => new IntCodeVM(Input).Run(1).First();

    protected override long? Part2()
        => new IntCodeVM(Input).Run(2).First();
}
