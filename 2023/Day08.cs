namespace AdventOfCode._2023;

using static Parser;
using Input = (char[] Directions, FrozenDictionary<string, string[]> Map);

class Day08 : Solution
{
    private static readonly Func<string, Input> ParseInput =
        from directions in AnyChar("LR").Many1().Token()
        from _ in NL.Many()
        from portals in (
            from src in Word + " = ("
            from l in Word + ", "
            from r in Word + ")"
            select (src, dirs: new[] { l, r })
        ).List(NL)
        select (directions, portals.ToFrozenDictionary(p => p.src, p => p.dirs));

    private static IEnumerable<long> WalkMap(Input m, string loc, Predicate<string> isTarget)
    {
        var seenTargets = new HashSet<string>();
        long steps = 0;
        while (true)
        {
            var d = m.Directions[steps++ % m.Directions.Length] == 'L' ? 0 : 1;
            loc = m.Map[loc][d];
            if (isTarget(loc))
            {
                yield return steps;
                if (!seenTargets.Add(loc))
                    yield break;
            }
        }
    }

    
    protected override long? Part1()
    {
        static long Solve(string input)
            => WalkMap(ParseInput(input), "AAA", n => n == "ZZZ").First();

        Assert(Solve(Sample()), 2);
        return Solve(Input);
    }


    protected override long? Part2()
    {
        var m = ParseInput(Input);

        return m.Map.Keys.Where(k => k.EndsWith("A"))
            .Select(s => WalkMap(m, s, n => n.EndsWith("Z")).ToList())
            .Aggregate(new HashSet<long> { 1 },
                (solutions, way) => solutions.SelectMany(s => way.Select(w => MathExt.LCM(s, w))).ToHashSet())
            .Min();
    }
}
