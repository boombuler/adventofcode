namespace AdventOfCode._2024;

class Day05 : Solution
{
    record Rule(int First, int Second);
    
    private int Repair(int[] update, Rule[] rules)
        => update.OrderBy(u => rules
                .Where(r => update.Contains(r.First))
                .CountBy(r => r.Second).ToDictionary().GetValueOrDefault(u))
            .ElementAt(update.Length / 2);

    private Parser<(IEnumerable<int> Valid, IEnumerable<int> Repaired)> InputParser =>
        from rules in (
            from first in Parser.Int + "|"
            from second in Parser.Int
            select new Rule(first, second)).List('\n') + "\n\n"
        from updates in Parser.Int.List(',').List('\n')
        let grp = updates.ToLookup(u => rules
            .Select(r => (I1: Array.IndexOf(u, r.First), I2: Array.IndexOf(u, r.Second)))
            .All(x => x.I1 < 0 || x.I2 < 0 || x.I1 < x.I2)
        )
        select (Valid: grp[true].Select(u => u[u.Length / 2]),
            Repaired: grp[false].Select(u => Repair(u, rules)));

    protected override long? Part1()
    {
        long Solve(string input) => InputParser.MustParse(input).Valid.Sum();
        
        Assert(Solve(Sample()), 143);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input) => InputParser.MustParse(input).Repaired.Sum();

        Assert(Solve(Sample()), 123);
        return Solve(Input);
    }
}
