namespace AdventOfCode._2022;

using System.Collections;

class Day14 : Solution
{
    private static long FillCave(string input, Func<Point2D, long, bool> finished)
    {
        const int CaveWidth = 1000;

        var rocks = (
            from line in input.Lines()
            from range in line.Split(" -> ").Select(Point2D.Parse).SlidingWindow(2)
            from pt in Point2D.Range(range[0], range[1])
            select pt
        ).ToList();
        var abys = rocks.Max(p => p.Y);
        var floor = abys + 2;

        var cave = new BitArray((int)((floor + 1) * CaveWidth));
        static int Index(Point2D pt) => (int)((pt.Y * CaveWidth) + pt.X);
        rocks.Select(Index).ForEach(i => cave[i] = true);
        Enumerable.Range(0, CaveWidth).ForEach(x => cave[Index((x, floor))] = true);

        int sand = 0;
        var directions = new Point2D[] { (0, 1), (-1, 1), (1, 1) };
        while (true)
        {
            var next = new Point2D(CaveWidth / 2, -1)
                .Unfold(p => directions.Select(d => d + p).FirstOrDefault(d => !cave[Index(d)]))
                .TakeWhile(n => n != null)
                .Last();
            sand++;

            if (finished(next, abys))
                return sand;
            cave[Index(next)] = true;
        }
    }

    protected override long? Part1()
    {
        static long Solve(string input)
            => FillCave(input, (pt, y) => pt.Y >= y) - 1;

        Assert(Solve(Sample()), 24);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input)
            => FillCave(input, (pt, _) => pt.Y == 0);

        Assert(Solve(Sample()), 93);
        return Solve(Input);
    }
}
