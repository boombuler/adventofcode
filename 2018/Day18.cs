namespace AdventOfCode._2018;

class Day18 : Solution
{
    enum State
    {
        Open,
        Tree,
        Lumberyard
    }

    private static State NextState(State cell, IEnumerable<State> neighbours)
        => cell switch
        {
            State.Open => neighbours.Count(n => n == State.Tree) >= 3 ? State.Tree : State.Open,
            State.Tree => neighbours.Count(n => n == State.Lumberyard) >= 3 ? State.Lumberyard : State.Tree,
            State.Lumberyard => neighbours.Any(l => l == State.Lumberyard) && neighbours.Any(l => l == State.Tree) ? State.Lumberyard : State.Open,
            _ => cell,
        };

    private static State[][] Read(string map)
        => map.Lines().Select(l => l.Select(c => c switch
        {
            '|' => State.Tree,
            '.' => State.Open,
            _ => State.Lumberyard
        }).ToArray()).ToArray();

    private static long Simulate(string map, int rounds)
    {
        var cur = Read(map);
        int size = cur.Length;
        var next = new State[size][];
        for (int r = 0; r < size; r++)
            next[r] = new State[size];

        var states = new Dictionary<string, int>();

        for (int i = 0; i < rounds; i++)
        {
            var sbMapStr = new StringBuilder();
            foreach (var pt in Point2D<int>.Range((0, 0), (size - 1, size - 1)))
            {
                var neigbours = pt.Neighbours(withDiagonal: true)
                    .Where(n => n.X >= 0 && n.Y >= 0 && n.Y < size && n.X < size)
                    .Select(n => cur[n.Y][n.X]);

                next[pt.Y][pt.X] = NextState(cur[pt.Y][pt.X], neigbours);
                sbMapStr.Append(next[pt.Y][pt.X] switch
                {
                    State.Lumberyard => '#',
                    State.Open => '.',
                    _ => '|'
                });
            }

            var mapStr = sbMapStr.ToString();
            if (!states.TryAdd(mapStr, i))
            {
                var roundSize = i - states[mapStr];
                var skip = ((rounds - i) / roundSize);
                if (skip > 0)
                    i += skip * roundSize;
            }

            (cur, next) = (next, cur);
        }

        var cells = cur.SelectMany(n => n);
        return cells.Count(n => n == State.Lumberyard) * cells.Count(n => n == State.Tree);
    }

    protected override long? Part1()
    {
        Assert(Simulate(Sample(), 10), 1147);
        return Simulate(Input, 10);
    }

    protected override long? Part2()
        => Simulate(Input, 1_000_000_000);
}
