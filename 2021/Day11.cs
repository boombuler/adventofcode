﻿namespace AdventOfCode._2021;

using Point = Point2D<int>;

class Day11 : Solution<int>
{
    private static (Dictionary<Point, int> NextState, int Flashes) StepRound(Dictionary<Point, int> cells)
    {
        cells = cells.ToDictionary(kvp => kvp.Key, kvp => kvp.Value + 1);

        var flashed = new HashSet<Point>();
        IEnumerable<Point> Flashing(Dictionary<Point, int> cells)
            => cells.Where(kvp => kvp.Value > 9 && !flashed.Contains(kvp.Key)).Select(kvp => kvp.Key);

        var bounds = new Rect2D<int>((0, 0), (9, 9));
        while (Flashing(cells).Any())
        {
            foreach (var pt in Flashing(cells))
            {
                flashed.Add(pt);
                foreach (var n in pt.Neighbours(withDiagonal: true).Where(bounds.Contains))
                    cells[n]++;
            }
        }
        foreach (var f in flashed)
            cells[f] = 0;
        return (cells, flashed.Count);
    }

    private static IEnumerable<int> Simulate(string input)
        => (State: input.Cells(c => c - '0'), Flashes: 0)
            .Unfold(c => StepRound(c.State))
            .Select(n => n.Flashes);

    protected override int Part1()
    {
        static int CountFlashes(string input, int rounds)
            => Simulate(input).Take(rounds).Sum();
        Assert(CountFlashes(Sample(), 10), 204);
        Assert(CountFlashes(Sample(), 100), 1656);
        return CountFlashes(Input, 100);
    }

    protected override int Part2()
    {
        static int FindSyncRound(string input)
            => Simulate(input).TakeWhile(n => n != 100).Count() + 1;

        Assert(FindSyncRound(Sample()), 195);
        return FindSyncRound(Input);
    }
}
