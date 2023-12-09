namespace AdventOfCode._2022;

class Day23 : Solution
{
    record Move(Point2D MoveDirection, params Point2D[] CheckDirections);
    private static readonly Move[] Moves = 
    [
        new ((0,-1), (-1, -1), (0, -1), (1,-1)),
        new ((0, 1), (-1, 1), (0, 1), (1, 1)),
        new ((-1, 0), (-1, -1), (-1, 0), (-1, 1)),
        new ((1, 0), (1, -1), (1, 0), (1, 1)),
    ];

    private static (long TotalRounds, long FreeSpace) MoveElves(string input, int maxRounds)
    {
        var elves = input.Cells().Where(c => c.Value == '#').Select(c => c.Key).ToHashSet();

        int r;
        for (r = 0; r < maxRounds; r++)
        {
            var moves =
                from elf in elves
                where elf.Neighbours(true).Any(elves.Contains)
                let newPos = (
                    from move in Enumerable.Range(0, Moves.Length).Select(m => Moves[(m + r) % Moves.Length])
                    where !move.CheckDirections.Select(d => d + elf).Any(elves.Contains)
                    select elf + move.MoveDirection
                ).FirstOrDefault(elf)
                where newPos != elf
                group (elf, newPos) by newPos into targetPositions
                where targetPositions.Count() == 1
                select targetPositions.First();
            bool moved = false;
            foreach(var (oldPos, newPos) in moves)
            {
                elves.Remove(oldPos);
                elves.Add(newPos);
                moved = true;
            }
            if (!moved)
                break;
        }
        var b = Rect2D.AABB(elves);
        return (r+1, (b.Width * b.Height) - elves.Count);
    }

    protected override long? Part1()
    {
        static long Solve(string input) => MoveElves(input, 10).FreeSpace;
        Assert(Solve(Sample("Small")), 25);
        Assert(Solve(Sample("Large")), 110);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input) => MoveElves(input, int.MaxValue).TotalRounds;
        Assert(Solve(Sample("Small")), 4);
        Assert(Solve(Sample("Large")), 20);
        return Solve(Input);
    }
}
