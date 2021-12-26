namespace AdventOfCode._2021;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AdventOfCode.Utils;

class Day11 : Solution<int>
{
    private static (ImmutableDictionary<Point2D, int> NextState, int Flashes) StepRound(ImmutableDictionary<Point2D, int> cells)
    {
        cells = cells.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value + 1);

        var flashed = new HashSet<Point2D>();
        IEnumerable<Point2D> Flashing(ImmutableDictionary<Point2D, int> cells)
            => cells.Where(kvp => kvp.Value > 9 && !flashed.Contains(kvp.Key)).Select(kvp => kvp.Key);
        while (Flashing(cells).Any())
        {
            foreach (var pt in Flashing(cells))
            {
                flashed.Add(pt);
                cells = cells.SetItems(pt.Neighbours(withDiagonal: true).Where(Point2D.InBounds((0, 0), (9, 9))).ToDictionary(p => p, p => cells[p] + 1));
            }
        }
        cells = cells.SetItems(flashed.ToDictionary(p => p, _ => 0));
        return (cells, flashed.Count);
    }

    private static IEnumerable<int> Simulate(string input)
        => (State: input.Cells(c => c - '0').ToImmutableDictionary(), Flashes: 0)
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
