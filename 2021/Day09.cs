namespace AdventOfCode._2021;

class Day09 : Solution
{
    protected override long? Part1()
    {
        var map = Input.Cells(c => c - '0');
        long result = 0;
        foreach (var pt in Point2D.Range(Point2D.Origin, map.Keys.Max()))
        {
            var n = pt.Neighbours().Where(map.ContainsKey).Select(k => map[k]);
            if (n.All(v => v > map[pt]))
                result += map[pt] + 1;
        }
        return result;
    }

    protected override long? Part2()
    {
        var map = Input.Cells().Where(kvp => kvp.Value != '9').Select(kvp => kvp.Key).ToHashSet();
        var open = new Stack<Point2D>();
        var basins = new MaxHeap<int>();

        while (map.Count > 0)
        {
            open.Push(map.First());
            int count = 0;
            while (open.TryPop(out var c))
            {
                if (!map.Remove(c))
                    continue;

                count++;
                foreach (var n in c.Neighbours())
                    open.Push(n);
            }
            basins.Push(count);
        }
        return basins.Pop() * basins.Pop() * basins.Pop();
    }
}
