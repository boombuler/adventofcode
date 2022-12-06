namespace AdventOfCode._2019;

class Day01 : Solution
{
    private long FuelNeeded(long mass) => (mass / 3) - 2;
    private long FuelNeededWithFuel(long mass) => mass.Unfold(FuelNeeded).TakeWhile(n => n > 0).Sum();

    protected override long? Part1()
        => Input.Lines().Select(long.Parse).Select(FuelNeeded).Sum();

    protected override long? Part2()
        => Input.Lines().Select(long.Parse).Select(FuelNeededWithFuel).Sum();
}
