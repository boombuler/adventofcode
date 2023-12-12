namespace AdventOfCode._2017;
using Point = Point2D<int>;
class Day22 : Solution
{
    public enum State
    {
        Clean = 0, //  nodes become weakened.
        Weakened = 1, // nodes become infected.
        Infected = 2, //  nodes become flagged.
        Flagged = 3, //  nodes become clean
    }
    enum TurnDirection : int { Left = -1, None = 0, Right = 1, Reverse = 2 }

    private static void HandleCarrier(string map, int burstCount, Func<State, (State, TurnDirection)> update)
    {
        var offset = new Point(map.Lines().First().Length / 2, map.Lines().Count() / 2);
        var nodeStates = map.Cells(filter: c => c == '#').ToDictionary(kvp => kvp.Key - offset, _ => State.Infected);

        var pos = Point.Origin;
        int direction = 0;
        var offsets = new Point[] { Point.Up, Point.Right, Point.Down, Point.Left };

        for (int i = 0; i < burstCount; i++)
        {
            if (!nodeStates.TryGetValue(pos, out State s))
                s = State.Clean;

            (var newState, var dir) = update(s);
            nodeStates[pos] = newState;
            direction = (direction + (int)dir) & 3;
            pos += offsets[direction];
        }
    }

    private static long VirusVersion1(string map, int burstCount)
    {
        long count = 0;
        HandleCarrier(map, burstCount, s =>
        {
            if (s == State.Clean)
            {
                count++;
                return (State.Infected, TurnDirection.Left);
            }
            return (State.Clean, TurnDirection.Right);
        });
        return count;
    }

    private static long VirusVersion2(string map, int burstCount)
    {
        long count = 0;
        HandleCarrier(map, burstCount, s =>
        {
            switch (s)
            {
                case State.Clean: return (State.Weakened, TurnDirection.Left);
                case State.Weakened:
                    count++;
                    return (State.Infected, TurnDirection.None);
                case State.Infected: return (State.Flagged, TurnDirection.Right);
                case State.Flagged: return (State.Clean, TurnDirection.Reverse);
                default: throw new InvalidOperationException();
            }
        });
        return count;
    }

    protected override long? Part1()
    {
        Assert(VirusVersion1(Sample(), 70), 41);
        Assert(VirusVersion1(Sample(), 10000), 5_587);
        return VirusVersion1(Input, 10_000);
    }

    protected override long? Part2()
    {
        Assert(VirusVersion2(Sample(), 100), 26);
        Assert(VirusVersion2(Sample(), 10_000_000), 2_511_944);
        return VirusVersion2(Input, 10_000_000);
    }
}
