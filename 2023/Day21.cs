namespace AdventOfCode._2023;

using Point = Point2D<int>;

class Day21 : Solution
{
    private static long Solve(string input, int steps)
    {
        var map = input.AsMap();

        var start = map.First(kvp => kvp.Value == 'S').Index;

        var seen = new HashSet<(Point pos, int steps)>();
        var queue = new Queue<(Point pos, int steps)>();
        queue.Enqueue((start, 0));
        while(queue.TryDequeue(out var cur))
        {
            if (!seen.Add((cur.pos, cur.steps % 2)) || cur.steps == steps)
                continue;

            foreach(var s in cur.pos.Neighbours())
            {
                var t = new Point(MathExt.Mod(s.X, map.Width), MathExt.Mod(s.Y, map.Height));
                if (map.GetValueOrDefault(t, '#') != '#')
                    queue.Enqueue((s, cur.steps + 1));
            }
        }

        return seen.Count(n => n.steps == (steps%2));
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample(), 6), 16);
        return Solve(Input, 64);
    }

    protected override long? Part2()
    {
        var steps = 26501365;
        var dim = Input.Lines().Count();
        var dimHalf = dim / 2;
        var (x, rem) = Math.DivRem(steps, dim);
        if (rem != dimHalf) 
            Error("Can not solve this!");
        
        return (long)MathExt.InterpolateFromSamples(
            Enumerable.Range(0, 3).Select(x => ((double)x, (double)Solve(Input, x * dim + dimHalf)))
        )(x);
    }    
}
