namespace AdventOfCode._2021;

class Day01 : Solution
{
    private long Solve(int windowSize)
        => Input.Lines().Select(long.Parse)
            .SlidingWindow(windowSize).Select(Enumerable.Sum)
            .Pairwise().Count(n => n.B > n.A);

    protected override long? Part1() => Solve(1);

    protected override long? Part2() => Solve(3);
}
