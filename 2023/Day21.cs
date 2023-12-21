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
        if (steps % dim != dimHalf) 
            Error("Can not solve this!");
        
        double x = (steps/dim) - dimHalf;
        double y0 = Solve(Input, 0*dim + dimHalf);
        double y1 = Solve(Input, 1*dim + dimHalf);
        double y2 = Solve(Input, 2*dim + dimHalf);
        double dy0 = y0;
        double dy1 = y1 - y0;
        double dy2 = y2 - y1;
        double ddy1 = dy2 - dy1;
        return (long)(dy0 + dy1 * x + x * (x - 1) / 2 * ddy1);
    }
}
