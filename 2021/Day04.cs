namespace AdventOfCode._2021;

class Day04 : Solution
{
    const int BOARD_SIZE = 5;
    class BingoBoard
    {
        private readonly Dictionary<long, Point2D> fCells;
        private readonly HashSet<Point2D> fMarked = [];

        public BingoBoard(IEnumerable<string> rows)
        {
            fCells = rows.SelectMany((l, y) =>
                l.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                 .Select((n, x) => new { Pt = new Point2D(x, y), N = long.Parse(n) })
            ).ToDictionary(x => x.N, x => x.Pt);
        }

        public long? MarkNumber(long n)
        {
            if (!fCells.TryGetValue(n, out var p) || !fMarked.Add(p))
                return null;

            if (Point2D.Range((0, p.Y), (BOARD_SIZE - 1, p.Y)).All(fMarked.Contains) || Point2D.Range((p.X, 0), (p.X, BOARD_SIZE - 1)).All(fMarked.Contains))
                return n * fCells.Where(kvp => !fMarked.Contains(kvp.Value)).Sum(kvp => kvp.Key);

            return null;
        }
    }

    private static IEnumerable<long> Scores(string input)
    {
        var (numbers, boardStrs) = input.Lines();
        var boards = boardStrs.Chunk(BOARD_SIZE + 1).Select(l => new BingoBoard(l.Skip(1))).ToList();

        foreach (var n in numbers.Split(',').Select(long.Parse))
        {
            var result = boards.Select(b => new { Board = b, Score = b.MarkNumber(n) }).Where(n => n.Score.HasValue).ToList();
            foreach (var res in result)
            {
                yield return res.Score.Value;
                boards.Remove(res.Board);
            }
        }
    }

    protected override long? Part1()
    {
        Assert(Scores(Sample()).First(), 4512);
        return Scores(Input).First();
    }

    protected override long? Part2()
    {
        Assert(Scores(Sample()).Last(), 1924);
        return Scores(Input).Last();
    }
}
