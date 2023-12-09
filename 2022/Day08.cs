namespace AdventOfCode._2022;

class Day08 : Solution
{
    private static IEnumerable<(int Height, IEnumerable<IEnumerable<int>> Views)> IterateViews(string input)
    {
        var trees = input.Cells(c => c - '0');
        var directions = new Point2D[] { (-1, 0), (0, -1), (1, 0), (0, 1) };
        return trees.Select(tree => 
            (tree.Value, directions.Select(d => tree.Key.Unfold(p => p + d).TakeWhile(trees.ContainsKey).Select(p => trees[p]))));
    }

    private static long CountVisibleTrees(string input)
        => IterateViews(input)
            .Count(tree => tree.Views.Any(v => v.All(t => t < tree.Height)));

    private static long GetMaxScenicScore(string input)
        => IterateViews(input)
            .Max(t => t.Views.Select(v => v.TakeUntil(h => h >= t.Height).Count()).Aggregate((a, b) => a * b));
    
    protected override long? Part1()
    {
        Assert(CountVisibleTrees(Sample()), 21);
        return CountVisibleTrees(Input);
    }

    protected override long? Part2()
    {
        Assert(GetMaxScenicScore(Sample()), 8);
        return GetMaxScenicScore(Input);
    }
}
