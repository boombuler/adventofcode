namespace AdventOfCode._2025;

using static Parser;

class Day12 : Solution
{
    private long Solve(string input)
    {
        var shapeLineParser = (AnyChar(".#", [0, 1]).Take(3) + "\n").Select(l => l.Sum());
        var shapeParser = (Int + ":\n").ThenR(shapeLineParser.Take(3).Select(l => l.Sum())) + "\n";
        var gridParser =
            from width in Int + "x"
            from height in Int + ":"
            from indices in Int.Token().Many()
            select (width * height, indices);
        var inputParser =
            from s in shapeParser.Many()
            from g in gridParser.List('\n')
            select (s, g);

        var (shapes, grids) = inputParser.MustParse(input);

        int ok = 0;
        foreach(var (size, indices) in grids)
        {
            var minRequiredArea = indices.Index().Sum(i => shapes[i.Index] * i.Item);
            if (minRequiredArea <= size)
                ok++;
        }

        return ok;
    }

    protected override long? Part1() => Solve(Input);
}
