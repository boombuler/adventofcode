namespace AdventOfCode._2022;

class Day01 : Solution
{
    private long GetCalorieSum(int elfCount)
    {
        var maxGroups = new PriorityQueue<long, long>(
            Enumerable.Range(0, elfCount)
                .Select(_ => (long.MinValue, long.MinValue))
        );
        Input.Split("\n\n")
            .Select(l => l.Lines().Sum(long.Parse))
            .ForEach(n => maxGroups.EnqueueDequeue(n, n));
        return maxGroups.UnorderedItems.Sum(n => n.Element);
    }

    protected override long? Part1() => GetCalorieSum(1);
    protected override long? Part2() => GetCalorieSum(3);
}
