namespace AdventOfCode._2022;

class Day12 : Solution
{
    private static int ShortestPath(string input, params char[] startPoints)
    {
        var map = input.Cells();
        var starts = map.Where(kvp => startPoints.Contains(kvp.Value)).Select(p=> p.Key);
        var end = map.Where(k => k.Value == 'E').Single().Key;
        var heights = map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value switch
        {
            'S' => 'a',
            'E' => 'z',
            var c => c
        } - 'a');

        var open = new Queue<(Point2D Pos, int Steps, int Height)>();
        foreach(var sp in starts)
            open.Enqueue((sp, 1, 0));

        while(open.TryDequeue(out var cur))
        {
            foreach(var n in cur.Pos.Neighbours())
            {
                if (heights.TryGetValue(n, out var nh) && (nh-cur.Height) <= 1)
                {
                    if (n == end)
                        return cur.Steps;
                    heights.Remove(n);
                    open.Enqueue((n, cur.Steps + 1, nh));
                }
            }
        }
        return -1;
    }

    protected override long? Part1()
    {
        static long Solve(string input) => ShortestPath(input, 'S');
        Assert(Solve(Sample()), 31);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input) => ShortestPath(input, 'S', 'a');
        Assert(Solve(Sample()), 29);
        return Solve(Input);
    }
}
