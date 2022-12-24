namespace AdventOfCode._2022;

class Day24 : Solution
{
    record State(int Time, Point2D Position);
    record Blizzard(Point2D Position, Point2D Direction);

    private (Point2D Start, Point2D End, Func<Point2D, Point2D, int, int> PathFinder) GetPathFinder(string input)
    {
        var map = input.Cells();
        var (startPos, endPos) = map.Where(m => m.Value == '.').Select(m => m.Key).MinMaxBy(m => m.Y);
        var blizzards = map.Select(c => new Blizzard(c.Key-(1,1), c.Value switch
        {
            '>' => (1, 0),
            '<' => (-1, 0),
            '^' => (0, -1),
            'v' => (0, 1),
            _ => Point2D.Origin
        })).Where(b=> b.Direction != Point2D.Origin).ToList();
        var (_, max) = Point2D.Bounds(map.Keys);

        var blizLoop = (int)MathExt.LCM(max.X - 1, max.Y - 1);
        var mapSize = max - (1, 1);
        var blizzardStates = Enumerable.Range(0, blizLoop)
            .Select(t => blizzards.Select(b => ((b.Position + (b.Direction * t) + (mapSize * t)) % mapSize) + (1, 1)).ToHashSet())
            .ToArray();

        return (startPos, endPos, (from, to, startTime) =>
        {
            var initialState = new State(startTime, from);
            var closed = new HashSet<State>();
            var open = new MinHeap<(State, long)>(ComparerBuilder<(State Item, long Cost)>.CompareBy(x => x.Cost));
            open.Push((initialState, 0));
            
            while (open.TryPop(out var current))
            {
                var (curState, _) = current;
                if (curState.Position == to)
                    return curState.Time;

                foreach (var next in PossibleTransitions(curState))
                {
                    var newCost = next.Time + next.Position.ManhattanDistance(endPos);
                    if (closed.Add(next with { Time = next.Time % blizLoop }))
                        open.Push((next, newCost));
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
        });
    }

    protected override long? Part1()
    {
        long Solve(string input)
        {
            var (start, end, findPath) = GetPathFinder(input);
            return findPath(start, end, 0);
        }
        Assert(Solve(Sample()), 18);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input)
        {
            var (start, end, findPath) = GetPathFinder(input);
            return findPath(start, end, findPath(end, start, findPath(start, end, 0)));
        }
        Assert(Solve(Sample()), 54);
        return Solve(Input);
    }
}
