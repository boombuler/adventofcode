namespace AdventOfCode._2017;

class Day19 : Solution<string, long?>
{
    private static readonly Point2D<int>[] Offsets =
    [
        Point2D<int>.Down,
        Point2D<int>.Left,
        Point2D<int>.Up,
        Point2D<int>.Right,
    ];

    private static IEnumerable<char> WalkPath(string data)
    {
        var grid = data.AsMap();

        var pos = new Point2D<int>(grid.Rows().First().Select((c, i) => (c, i)).Where(i => i.c != ' ').Select(i => i.i).First(), 0);
        yield return grid.GetValueOrDefault(pos, ' ');

        var direction = 0; // Down;
        while (true)
        {
            pos += Offsets[direction];
            if (!grid.TryGetValue(pos, out var chr) || chr == ' ')
                yield break;

            yield return chr;

            if (chr == '+')
            {
                direction ^= 0b_01; // turn 90°
                if (grid.GetValueOrDefault(pos + Offsets[direction], ' ') == ' ')
                    direction ^= 0b_10; // turn 180° 
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
