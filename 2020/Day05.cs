﻿namespace AdventOfCode._2020;

class Day05 : Solution
{
    private int ToSeatId(string position)
        => position.Aggregate(0, (val, c) => (val << 1) | (c is 'B' or 'R' ? 1 : 0));

    private IEnumerable<int> Seats => Input.Lines().Select(ToSeatId);
    protected override long? Part1() => Seats.Max();
    protected override long? Part2()
        => Seats.Order().Pairwise().First(s => (s.B - s.A) == 2).A + 1;
}
