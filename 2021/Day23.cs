namespace AdventOfCode._2021;

using System.Diagnostics.CodeAnalysis;

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
        {
            for (int i = 0; i < x.Length; i++)
                if (!Equals(x[i], y[i]))
                    return false;
            return true;
        }
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

    private static int RoomPos(int roomNo) => (roomNo * 2) + 1;

    private static IEnumerable<(ImmutableArray<Amphipod> Positions, int Costs)> GetPossibleStates(ImmutableArray<Amphipod> positions)
    {
        const int HALLWAY = 0;
        var hallwayAmps = 0;
        var minRoomPos = new int[5];
        Array.Fill(minRoomPos, positions.Length);
        var typesPerRoom = new int[5] { 0, 1, 2, 4, 8 };
        foreach (var a in positions)
        {
            if (a.Room == HALLWAY)
                hallwayAmps |= 1 << a.Pos;
            minRoomPos[a.Room] = minRoomPos[a.Room] < a.Pos ? minRoomPos[a.Room] : a.Pos;
            typesPerRoom[a.Room] |= 1 << a.Type;
        }

        foreach (var a in positions)
        {
            if (a.Room == HALLWAY)
            {
                if (typesPerRoom[a.TargetRoom] != (1 << a.Type))
                    continue; // There are other types in target room. 

                var targetHallwayPos = RoomPos(a.TargetRoom);
                var hwMin = targetHallwayPos;
                var hwMax = a.Pos + Math.Sign(targetHallwayPos - a.Pos);
                (hwMin, hwMax) = hwMin > hwMax ? (hwMax, hwMin) : (hwMin, hwMax);
                var hwMask = 0;
                for (int i = hwMin; i <= hwMax; i++)
                    hwMask |= (1 << i);

                if ((hallwayAmps & hwMask) != 0)
                    continue; // No other between current pos and target room.

                var targetPos = (positions.Length / 4) - positions.Count(o => o.Room == a.TargetRoom);
                var cost = (targetPos + Math.Abs(targetHallwayPos - a.Pos)) * a.MoveCost;

                yield return (positions.Replace(a, new Amphipod(a.TargetRoom, targetPos - 1, a.Type)), cost);
            }
            else //  (a.Room != HALLWAY)
            {
                if (minRoomPos[a.Room] < a.Pos)
                    continue; // Blocked
                if (a.InCorrectRoom && typesPerRoom[a.Room] == (1 << a.Type))
                    continue; // No reason to leave the room.

                var roomHallwayPos = (a.Room * 2) + 1;

                var pos = roomHallwayPos - 1;
                while (((hallwayAmps & (1 << pos)) == 0) && pos >= 1)
                {
                    var cost = (1 + a.Pos + (roomHallwayPos - pos)) * a.MoveCost;
                    yield return (positions.Replace(a, new Amphipod(HALLWAY, pos, a.Type)), cost);
                    if (pos-- > 3)
                        pos--;
                }

                pos = roomHallwayPos + 1;
                while (((hallwayAmps & (1 << pos)) == 0) && pos <= 11)
                {
                    var cost = (1 + a.Pos + (pos - roomHallwayPos)) * a.MoveCost;
                    yield return (positions.Replace(a, new Amphipod(HALLWAY, pos, a.Type)), cost);
                    if (pos++ < 9)
                        pos++;
                }
            }
        }
    }

    private static long? Solve(string map, bool expand)
    {
        var cells = map.Cells();
        var amphipods = Enumerable.Range(2, 2).SelectMany(r =>
            Enumerable.Range(1, 4).Select(c => new Amphipod(c, r - 2, cells[(1 + (c * 2), r)] - 'A'))
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
        var open = new MinHeap<(ImmutableArray<Amphipod> Positions, int TotalCost)>(
            Comparer<(ImmutableArray<Amphipod>, int TotalCost)>.Create((a,b) => a.TotalCost - b.TotalCost)
        );
        open.Push((amphipods, 0));
        while (open.TryPop(out var current))
        {
            var (pos, totalCost) = current;
            if (!seen.Add(pos))
                continue;

            if (pos.All(p => p.InCorrectRoom))
                return totalCost;

            foreach (var (newPos, extraCost) in GetPossibleStates(pos))
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
