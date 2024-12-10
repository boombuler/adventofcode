namespace AdventOfCode._2024;

using Point = Point2D<int>;

class Day10 : Solution
{
    private long FindTargets(StringMap<int> map, Point start, Func<Point, bool> filter)
    {
        var open = new Queue<Point>([start]);
        long count = 0;
        while (open.TryDequeue(out var cur))
        {
            if (!filter(cur))
                continue;

            var curVal = map[cur];
            if (curVal == 9)
                count++;
            else
            {
                cur.Neighbours()
                    .Where(n => map.TryGetValue(n, out var val) && (val - curVal) == 1)
                    .ForEach(open.Enqueue);
            }
        }
        return count;
    }

    private long CountPaths<T>(string input, Func<T> init, Func<T, Point, bool> filter)
    {
        var map = input.AsMap(c => c - '0');
        return map.Where(x => x.Value == 0).Select(x => x.Index)
            .Sum(s => { var t = init(); return FindTargets(map, s, p => filter(t, p)); });
    }

    protected override long? Part1() 
    {
        long Solve(string input) 
            => CountPaths(input, () => new HashSet<Point>(), (h, p) => h.Add(p));

        Assert(Solve(Sample()), 36);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input) 
            => CountPaths(input, () => Unit.Value, (_, _) => true);

        Assert(Solve(Sample()), 81);
        return Solve(Input);
    }
}
