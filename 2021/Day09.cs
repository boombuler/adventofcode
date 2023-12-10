namespace AdventOfCode._2021;

class Day09 : Solution
{
    protected override long? Part1()
    {
        var map = Input.AsMap(c => c - '0');
        long result = 0;
        foreach (var (pt, val) in map)
        {
            var n = pt.Neighbours().Where(map.Contains).Select(k => map[k]);
            if (n.All(v => v > val))
                result += val + 1;
        }
        return result;
    }

    protected override long? Part2()
    {
        var map = Input.Cells(filter: v => v != '9').Keys.ToHashSet();
        var open = new Stack<Point2D>();
        var basins = new PriorityQueue<long, long>();
        basins.EnqueueRange([(0, -1), (0, -1), (0, -1)]);

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
            basins.EnqueueDequeue(count, count);
        }
        return basins.Dequeue() * basins.Dequeue() * basins.Dequeue();
    }
}
