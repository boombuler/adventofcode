namespace AdventOfCode._2016;

using System.Buffers;

class Day11 : Solution
{
    record struct Items(int Chips, int Generators)
    {
        public static readonly Items Empty = new(0, 0);

        public readonly int Count => NumberOfSetBits(Chips) + NumberOfSetBits(Generators);
        public readonly int GetElements(Items[] buffer)
        {
            int r = 0;
            var c = Chips;
            var g = Generators;
            for (int i = 0; c != 0 || g != 0; i++)
            {
                var mask = 1 << i;
                if ((c & mask) != 0)
                    buffer[r++] = new Items(mask, 0);
                if ((g & mask) != 0)
                    buffer[r++] = new Items(0, mask);
                mask = ~mask;
                c &= mask;
                g &= mask;
            }
            return r;
        }

        public readonly bool IsValidFloor
        {
            get
            {
                bool hasGen = Generators != 0;
                bool missingGen = (Chips & ~Generators) != 0;
                return !hasGen || !missingGen;
            }
        }

        static int NumberOfSetBits(int i)
        {
            i -= ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }
    }

    class State
    {
        private static readonly IEnumerable<State> NONE = Enumerable.Empty<State>();
        private readonly ImmutableArray<Items> Floors;
        private readonly int ElevatorPos;
        public int NumberOfMoves { get; }
        public State(ImmutableArray<Items> floors)
            : this(floors, 0, 0)
        {
        }
        private State(ImmutableArray<Items> floors, int moveCount, int elevatorPos)
        {
            Floors = floors;
            NumberOfMoves = moveCount;
            ElevatorPos = elevatorPos;
        }

        public bool Success()
        {
            if (ElevatorPos != Floors.Length - 1)
                return false;
            for (int i = 0; i < (Floors.Length - 1); i++)
                if (Floors[i].Chips != 0 || Floors[i].Generators != 0)
                    return false;
            return true;
        }

        private State Move(int dir, int chips, int generators)
        {
            var curFloor = Floors[ElevatorPos];
            curFloor = new Items(curFloor.Chips & ~chips, curFloor.Generators & ~generators);

            if (!curFloor.IsValidFloor)
                return null;

            var other = Floors[ElevatorPos + dir];
            other = new Items(other.Chips | chips, other.Generators | generators);

            if (!other.IsValidFloor)
                return null;

            var newFloors = Floors.SetItem(ElevatorPos, curFloor).SetItem(ElevatorPos + dir, other);
            return new State(newFloors, NumberOfMoves + 1, ElevatorPos + dir);
        }

        private IEnumerable<State> MoveOneItem(int dir, Items[] items, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var elem = items[i];
                var move = Move(dir, elem.Chips, elem.Generators);
                if (move != null)
                    yield return move;
            }
        }

        private IEnumerable<State> MoveTwoItems(int dir, Items[] items, int count)
        {
            var len = count;
            for (int i1 = 0; i1 < len; i1++)
            {
                var (c1, g1) = items[i1];
                for (int i2 = i1 + 1; i2 < len; i2++)
                {
                    var (c2, g2) = items[i2];
                    var m = Move(dir, c1 | c2, g1 | g2);
                    if (m != null)
                        yield return m;
                }
            }
        }

        public IEnumerable<State> GetValidMoves()
        {
            var moves = NONE;
            var buf = ArrayPool<Items>.Shared.Rent(Floors[ElevatorPos].Count);
            try
            {
                var elemCount = Floors[ElevatorPos].GetElements(buf);

                void AppendGenerator(int dir, Func<int, Items[], int, IEnumerable<State>> genA, Func<int, Items[], int, IEnumerable<State>> genB)
                {
                    var items = genA(dir, buf, elemCount);
                    if (items.Any())
                        moves = moves.Concat(items);
                    else
                        moves = moves.Concat(genB(dir, buf, elemCount));
                }

                if (ElevatorPos < (Floors.Length - 1))
                    AppendGenerator(+1, MoveTwoItems, MoveOneItem);
                if (ElevatorPos > 0)
                    AppendGenerator(-1, MoveOneItem, MoveTwoItems);
            }
            finally
            {
                ArrayPool<Items>.Shared.Return(buf);
            }
            return moves;
        }

        public override int GetHashCode()
        {
            var result = ElevatorPos.GetHashCode();
            for (int i = 0; i < Floors.Length; i++)
                result = (result * 23) + Floors[i].GetHashCode();
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is State other)
                return ElevatorPos == other.ElevatorPos && Floors.SequenceEqual(other.Floors);
            return false;
        }
    }

    private static long Solve(string notes)
    {
        var itemTypes = new List<string>();

        var floors = notes.Lines().Select(note =>
        {
            note = note[(note.IndexOf("contains") + "contains".Length)..].Replace(" and ", ",");
            var itms = note.Split(',')
                .Select(itm => itm.Trim(' ', '.'))
                .Where(itm => itm.StartsWith("a ") || itm.StartsWith("an "))
                .Select(itm => itm[2..].Split(' ', '-'))
                .Select(parts => new { Element = parts.First(), Generator = parts.Last() == "generator" });
            int chips = 0, generatos = 0;
            foreach (var itm in itms)
            {
                var id = itemTypes.IndexOf(itm.Element);
                if (id == -1)
                {
                    id = itemTypes.Count;
                    itemTypes.Add(itm.Element);
                }
                var mask = 1 << id;
                if (itm.Generator)
                    generatos |= mask;
                else
                    chips |= mask;
            }
            return new Items(chips, generatos);
        }).ToImmutableArray();

        var states = new Queue<State>();
        states.Enqueue(new State(floors));
        var checkedMoves = new HashSet<State>();
        while (states.TryDequeue(out var state))
        {
            if (state.Success())
                return state.NumberOfMoves;
            else
            {
                foreach (var move in state.GetValidMoves())
                {
                    if (checkedMoves.Add(move))
                        states.Enqueue(move);
                }
            }
        }

        return long.MaxValue;
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample()), 11);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        const string FIRST_FLOOR_PREFIX = "The first floor contains";
        const string EXTRA_COMPONENTS = " a elerium generator, a elerium-compatible microchip, a dilithium generator, a dilithium-compatible microchip,";
        var input = Input.Replace(FIRST_FLOOR_PREFIX, FIRST_FLOOR_PREFIX + EXTRA_COMPONENTS);
        return Solve(input);
    }
}
