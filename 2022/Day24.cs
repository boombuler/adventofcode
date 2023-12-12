namespace AdventOfCode._2022;

using Point = Point2D<int>;

class Day24 : Solution
{
    record State(int Time, Point Position);
    record Blizzard(Point Position, Point Direction);

    private static (Point Start, Point End, Func<Point, Point, int, int> PathFinder) GetPathFinder(string input)
    {
        var map = input.Cells();
        var (startPos, endPos) = map.Where(m => m.Value == '.').Select(m => m.Key).MinMaxBy(m => m.Y);
        var blizzards = map.Select(c => new Blizzard(c.Key - (1, 1), Point.DirectionFromArrow(c.Value))).Where(b => b.Direction != Point.Origin).ToList();
        var (_, max) = Rect2D<int>.AABB(map.Keys);

        var blizLoop = (int)MathExt.LCM(max.X - 1, max.Y - 1);
        var mapSize = max - (1, 1);
        var blizzardStates = Enumerable.Range(0, blizLoop)
            .Select(t => blizzards.Select(b => ((b.Position + (b.Direction * t) + (mapSize * t)) % mapSize) + (1, 1)).ToHashSet())
            .ToArray();

        return (startPos, endPos, (from, to, startTime) =>
        {
            var initialState = new State(startTime, from);
            var closed = new HashSet<State>();
            var open = new PriorityQueue<State, long>();
            open.Enqueue(initialState, 0);

            while (open.TryDequeue(out var curState, out _))
            {
                if (curState.Position == to)
                    return curState.Time;

                foreach (var next in PossibleTransitions(curState))
                {
                    var newCost = next.Time + next.Position.ManhattanDistance(endPos);
                    if (closed.Add(next with { Time = next.Time % blizLoop }))
                        open.Enqueue(next, newCost);
                }
            }

            return -1;

            IEnumerable<State> PossibleTransitions(State s)
            {
                var t = s.Time + 1;
                var bliz = blizzardStates[t % blizLoop];

                return s.Position.Neighbours()
                    .Where(p => p == to || (p.X >= 1 && p.Y >= 1 && p.X < max.X && p.Y < max.Y))
                    .Append(s.Position)
                    .Where(p => !bliz.Contains(p))
                    .Select(p => new State(t, p));
            }
        }
        );
    }

    protected override long? Part1()
    {
        static long Solve(string input)
        {
            var (start, end, findPath) = GetPathFinder(input);
            return findPath(start, end, 0);
        }
        Assert(Solve(Sample()), 18);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input)
        {
            var (start, end, findPath) = GetPathFinder(input);
            return findPath(start, end, findPath(end, start, findPath(start, end, 0)));
        }
        Assert(Solve(Sample()), 54);
        return Solve(Input);
    }
}
