namespace AdventOfCode._2024;

class Day20 : Solution
{
    private static long Solve(string input, int cheatTime, int minTimeSaved)
    {
        var map = input.AsMap();
        var start = map.Find('S');

        var path = (Cur: start, Last: start).Unfold(from =>
            (from.Cur?.Neighbours().FirstOrDefault(n => n != from.Last && map.GetValueOrDefault(n) != '#'), from.Cur)
        ).Select(n => n.Cur).TakeWhile(n => n != null).Prepend(start).Select(n => n!).ToList();

        int count = 0;
        for (int s = 0; s < path.Count - minTimeSaved; s++)
        {
            var sp = path[s];
            for (int e = s+minTimeSaved; e < path.Count; e++)
            {
                var md = sp.ManhattanDistance(path[e]);
                if (md <= cheatTime && (e - s - md) >= minTimeSaved)
                    count++;
            }
        }

        return count;
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample(),2, 64), 1);
        Assert(Solve(Sample(), 2, 40), 2);
        Assert(Solve(Sample(), 2, 20), 5);
        return Solve(Input, 2, 100);
    }

    protected override long? Part2()
    {
        Assert(Solve(Sample(), 20, 76), 3);
        Assert(Solve(Sample(), 20, 74), 7);
        return Solve(Input, 20, 100);
    }
}
