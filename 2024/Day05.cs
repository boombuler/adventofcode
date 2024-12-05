namespace AdventOfCode._2024;

class Day05 : Solution
{
    private IEnumerable<int> Repair(int[] update, ILookup<int, int> rules)
        => update.OrderBy(update.SelectMany(u => rules[u]).CountBy(u => u).ToDictionary().GetValueOrDefault);

    private Parser<(IEnumerable<int> Valid, IEnumerable<int> Repaired)> InputParser =>
        from ruleList in (
            from first in Parser.Int + "|"
            from second in Parser.Int
            select (first, second)).List('\n') + "\n\n"
        let rules = ruleList.ToHashSet()
        let repairLookup = ruleList.ToLookup(r => r.first, r => r.second)
        from updates in Parser.Int.List(',').List('\n')
        let grp = updates.ToLookup(u => !u.Pairwise((a, b) => (b, a)).Any(rules.Contains))
        select (Valid: grp[true].Select(u => u[u.Length / 2]),
            Repaired: grp[false].Select(u => Repair(u, repairLookup).ElementAt(u.Length /2)));

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
