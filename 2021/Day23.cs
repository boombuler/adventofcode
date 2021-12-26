using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AdventOfCode._2021
{
    class Day23 : Solution
    {
        record Amphipod(int Room, int Pos, int Type)
        {
            public int TargetRoom => Type + 1;
            public bool InCorrectRoom => Room == TargetRoom;
            public int MoveCost => (int)Math.Pow(10, Type);
        }

        class AmphipodArrayComparer : IEqualityComparer<ImmutableArray<Amphipod>>
        {
            public bool Equals(ImmutableArray<Amphipod> x, ImmutableArray<Amphipod> y)
                => x.SequenceEqual(y);
            public int GetHashCode([DisallowNull] ImmutableArray<Amphipod> obj)
            {
                unchecked
                {
                    int hash = 17;
                    foreach (var item in obj)
                        hash = hash * 23 + item.GetHashCode();
                    return hash;
                }
            }
        }

        private int RoomPos(int roomNo) => (roomNo * 2) + 1;

        private IEnumerable<(ImmutableArray<Amphipod> Positions, long Costs)> GetPossibleStates(ImmutableArray<Amphipod> positions)
        {
            const int HALLWAY = 0;
            var hallWayAmps = positions.Where(o => o.Room == HALLWAY).Select(o => o.Pos).ToArray();
            foreach (var a in positions)
            {
                if (a.Room == HALLWAY)
                {
                    if (positions.Any(other => other.Room == a.TargetRoom && other.Type != a.Type))
                        continue; // There are other types in target room. 

                    var targetHallwayPos = RoomPos(a.TargetRoom);
                    var hwMin = targetHallwayPos;
                    var hwMax = a.Pos + Math.Sign(targetHallwayPos - a.Pos);
                    (hwMin, hwMax) = hwMin > hwMax ? (hwMax, hwMin) : (hwMin, hwMax);

                    if (hallWayAmps.Any(other => other >= hwMin && other <= hwMax))
                        continue; // No other between current pos and target room.

                    var targetPos = (positions.Length / 4) - positions.Count(o => o.Room == a.TargetRoom);
                    var cost = (targetPos + Math.Abs(targetHallwayPos - a.Pos)) * a.MoveCost;

                    yield return (positions.Replace(a, new Amphipod(a.TargetRoom, targetPos - 1, a.Type)), cost);
                }
                else //  (a.Room != HALLWAY)
                {
                    if (positions.Any(other => other.Room == a.Room && other.Pos < a.Pos))
                        continue; // Blocked
                    if (a.InCorrectRoom && positions.Where(p => p.Room == a.Room).All(p => p.Type == a.Type))
                        continue; // No reason to leave the room.

                    var roomHallwayPos = (a.Room * 2) + 1;

                    var pos = roomHallwayPos - 1;
                    while (!hallWayAmps.Contains(pos) && pos >= 1)
                    {
                        var cost = (1 + a.Pos + (roomHallwayPos - pos)) * a.MoveCost;
                        yield return (positions.Replace(a, new Amphipod(HALLWAY, pos, a.Type)), cost);
                        if (pos-- > 3)
                            pos--;
                    }

                    pos = roomHallwayPos + 1;
                    while (!hallWayAmps.Contains(pos) && pos <= 11)
                    {
                        var cost = (1 + a.Pos + (pos - roomHallwayPos)) * a.MoveCost;
                        yield return (positions.Replace(a, new Amphipod(HALLWAY, pos, a.Type)), cost);
                        if (pos++ < 9)
                            pos++;
                    }
                }
            }
        }

        private long? Solve(string map, bool expand)
        {
            var cells = map.Cells();
            var amphipods = Enumerable.Range(2, 2).SelectMany(r =>
                Enumerable.Range(1, 4).Select(c => new Amphipod(c, r-2,  cells[(1 + (c * 2), r)] - 'A'))
            ).ToImmutableArray();
            if (expand)
            {
                amphipods = amphipods.Select(a => a with { Pos = a.Pos * 3 }).ToImmutableArray();
                amphipods = amphipods.AddRange(new[]
                {
                    new Amphipod(1, 1, 3), new Amphipod(2, 1, 2), new Amphipod(3, 1, 1), new Amphipod(4, 1, 0),
                    new Amphipod(1, 2, 3), new Amphipod(2, 2, 1), new Amphipod(3, 2, 0), new Amphipod(4, 2, 2),
                });
            }

            var seen = new HashSet<ImmutableArray<Amphipod>>(new AmphipodArrayComparer());
            var open = new MinHeap<(ImmutableArray<Amphipod> Positions, long TotalCost)>(
                ComparerBuilder<(ImmutableArray<Amphipod> Positions, long TotalCost)>.CompareBy(s => s.TotalCost)
            );
            open.Push((amphipods, 0));
            while(open.TryPop(out var current))
            {
                var (pos, totalCost) = current;
                if (!seen.Add(pos))
                    continue;

                if (pos.All(p => p.InCorrectRoom))
                    return totalCost;

                foreach(var (newPos, extraCost) in GetPossibleStates(pos))
                    open.Push((newPos, extraCost + totalCost));
            }

            return null;
        }

        protected override long? Part1()
        {
            Assert(Solve(Sample(), false), 12521);
            return Solve(Input, false);
        }

        protected override long? Part2()
        {
            Assert(Solve(Sample(), true), 44169);
            return Solve(Input, true);
        }
    }
}
