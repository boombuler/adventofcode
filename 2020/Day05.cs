using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2020
{
    class Day05 : Solution
    {   
        private int ToSeatId(string position)
            => position.Aggregate(0, (val, c) => (val << 1) | (c == 'B' || c == 'R' ? 1 : 0));

        private int FindOwnSeat(List<int> sortedSeats)
        {
            int prev = sortedSeats.First();
            foreach(var next in sortedSeats.Skip(1))
            {
                if (next - prev == 2)
                    return next - 1;
                prev = next;
            }
            return -1;
        }

        private List<int> SortedSeats => Input.Lines().Select(ToSeatId).OrderBy(x => x).ToList();
        protected override long? Part1() => SortedSeats.Last();
        protected override long? Part2() => FindOwnSeat(SortedSeats);
    }
}
