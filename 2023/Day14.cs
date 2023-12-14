namespace AdventOfCode._2023;

using Point = Point2D<int>;
class Day14 : Solution
{
    private static long CalculateTotalLoad(string input, int cycleCount, Point[] directions)
    {
        var map = input.AsMap();
        var rockPos = map.Where(n => n.Value == 'O').Select(n => n.Index).ToList();
        var seenStates = new Dictionary<string, int>();
        for (int i = 0; i < cycleCount; i++)
        {
            foreach (var direction in directions)
            {
                var next = new List<Point>(rockPos.Count);
                foreach (var pOld in rockPos.OrderBy(n => (n.X*-direction.X, n.Y*-direction.Y)))
                {
                    var pNew = pOld;
                    while (map.GetValueOrDefault(pNew + direction, '#') == '.')
                        pNew += direction;
                    map[pOld] = '.';
                    map[pNew] = 'O';
                    next.Add(pNew);
                }
                rockPos = next;
            }

            var h = string.Join("|", rockPos);
            if (seenStates.TryGetValue(h, out var lastTime))
            {
                var cycle = (i - lastTime);
                i += ((cycleCount - i) / cycle) * cycle;
            }
            seenStates[h] = i;
        }
        return rockPos.Sum(p => map.Height - p.Y);
    }
    protected override long? Part1()
    {
        static long Solve(string s)
            => CalculateTotalLoad(s, 1, [Point.Up]);

        Assert(Solve(Sample()), 136);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string s) 
            => CalculateTotalLoad(s, 1_000_000_000, [Point.Up, Point.Left, Point.Down, Point.Right]);

        Assert(Solve(Sample()), 64);
        return Solve(Input);
    }
}
