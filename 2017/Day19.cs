namespace AdventOfCode._2017;

class Day19 : Solution<string, long?>
{
    private static readonly Point2D[] Offsets = new Point2D[]
    {
        (0, 1),     // Down
        (-1, 0),    // Left
        (0, -1),    // Up
        (1, 0)      // Right
    };

    private static IEnumerable<char> WalkPath(string data)
    {
        var grid = data.Lines().Select(l => l.ToArray()).ToArray();
        char get(Point2D pt)
        {
            if (pt.X < 0 || pt.Y < 0 || pt.Y >= grid.Length)
                return ' ';
            var row = grid[pt.Y];
            return (pt.X >= row.Length) ? ' ' : row[pt.X];
        }

        var pos = new Point2D(grid[0].Select((c, i) => (c, i)).Where(i => i.c != ' ').Select(i => i.i).First(), 0);
        yield return get(pos);

        var direction = 0; // Down;
        while (true)
        {
            pos += Offsets[direction];
            var chr = get(pos);
            if (chr == ' ')
                yield break;

            yield return chr;

            if (chr == '+')
            {
                direction ^= 1; // turn 90°
                if (get(pos + Offsets[direction]) == ' ')
                    direction ^= 2; // turn 180° 
            }
        }
    }

    private static string GetText(string data)
        => new(WalkPath(data).Where(char.IsLetter).ToArray());

    protected override string Part1()
    {
        Assert(GetText(Sample()), "ABCDEF");
        return GetText(Input);
    }

    protected override long? Part2()
    {
        Assert(WalkPath(Sample()).Count(), 38);
        return WalkPath(Input).Count();
    }
}
