namespace AdventOfCode._2023;

using static Parser;

class Day08 : Solution
{
    private static readonly Func<string, (int[] Directions, FrozenDictionary<string, string[]> Map)> ParseInput =
        from directions in (Char('L', 0) | Char('R', 1)).Many1()
        let Name = Letter.Take(3).Text()
        from _ in NL.Many()
        from portals in (
            from src in Name + " = ("
            from l in Name + ", "
            from r in Name + ")"
            select (src, dirs: new[] { l, r })
        ).List(NL)
        select (directions, portals.ToFrozenDictionary(p => p.src, p => p.dirs));

    private static long WalkMap(string input, Func<string, bool> isStart, Func<string, bool> isTarget)
    {
        var (directions, map) = ParseInput(input);
        return map.Keys.Where(isStart)
            .Select(start =>
                (Steps: 0L, Location: start)
                    .Unfold(n => (n.Steps + 1, map[n.Location][directions[n.Steps % directions.Length]]))
                    .First(n => isTarget(n.Location)).Steps
            ).Aggregate(MathExt.LCM);
    }

    protected override long? Part1()
    {
        static long Solve(string input)
            => WalkMap(input, n => n == "AAA", n => n == "ZZZ");

        Assert(Solve(Sample()), 2);
        return Solve(Input);
    }

    protected override long? Part2()
        => WalkMap(Input, n => n[2] == 'A', n => n[2] == 'Z');
}
