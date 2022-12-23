namespace AdventOfCode._2022;

class Day23 : Solution
{
    record Move(Point2D MoveDirection, params Point2D[] CheckDirections);
    private static readonly Move[] Moves = new[]
    {
        new Move((0,-1), (-1, -1), (0, -1), (1,-1)),
        new Move((0, 1), (-1, 1), (0, 1), (1, 1)),
        new Move((-1, 0), (-1, -1), (-1, 0), (-1, 1)),
        new Move((1, 0), (1, -1), (1, 0), (1, 1)),
    };

    private (long TotalRounds, long FreeSpace) MoveElves(string input, int maxRounds)
    {
        var elves = input.Cells().Where(c => c.Value == '#').Select(c => c.Key).ToHashSet();

        int r;
        for (r = 0; r < maxRounds; r++)
        {
            var proposedMoves = new Dictionary<Point2D, Point2D>();
            foreach(var elf in elves.Where(e => e.Neighbours(true).Any(elves.Contains)))
            {
                for (int m = 0; m < Moves.Length; m++)
                {
                    var mv = Moves[(m + r) % Moves.Length];
                    if (mv.CheckDirections.Select(d => elf + d).Any(elves.Contains))
                        continue;
                    var newPos = elf + mv.MoveDirection;
                    if (!proposedMoves.TryAdd(newPos, elf))
                        proposedMoves.Remove(newPos);
                    break;
                }
            }
            if (proposedMoves.Count == 0)
                break;
            foreach (var (newPos, oldPos) in proposedMoves)
            {
                elves.Remove(oldPos);
                elves.Add(newPos);
            }
        }
        var b = Point2D.Bounds(elves);
        var dx = b.Max.X - b.Min.X + 1;
        var dy = b.Max.Y - b.Min.Y + 1;
        return (r+1, (dx * dy) - elves.Count);
    }

    protected override long? Part1()
    {
        long Solve(string input) => MoveElves(input, 10).FreeSpace;
        Assert(Solve(Sample("Small")), 25);
        Assert(Solve(Sample("Large")), 110);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input) => MoveElves(input, int.MaxValue).TotalRounds;
        Assert(Solve(Sample("Small")), 4);
        Assert(Solve(Sample("Large")), 20);
        return Solve(Input);
    }
}
