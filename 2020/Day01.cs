namespace AdventOfCode._2020;

class Day01 : Solution
{
    private static int? Solve(int[] items, int factorCount, int targetSum = 2020)
        => items.Combinations(factorCount).First(itms => itms.Sum() == targetSum).Aggregate((a, b) => a * b);

    private int[] Items => Input.Lines().Select(int.Parse).Order().ToArray();
    protected override long? Part1() => Solve(Items, 2);
    protected override long? Part2() => Solve(Items, 3);

}
