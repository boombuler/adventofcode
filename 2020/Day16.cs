namespace AdventOfCode._2020;

class Day16 : Solution
{
    record Rule(string Name, long Min1, long Min2, long Max1, long Max2)
    {
        public bool Valid(long v) => (v >= Min1 && v <= Max1) || (v >= Min2 && v <= Max2);
    }

    private static readonly Func<string, Rule> ParseRule =
        new Regex(@"(?<Name>[ \w]+): (?<Min1>\d+)-(?<Max1>\d+) or (?<Min2>\d+)-(?<Max2>\d+)").ToFactory<Rule>();

    private static IEnumerable<Rule> GetRules(string input)
    {
        foreach (var line in input.Lines())
        {
            var m = ParseRule(line);
            if (m == null)
                yield break;
            yield return m;
        }
    }

    private static IEnumerable<long[]> OtherTickets(string input)
    {
        foreach (var line in input.Lines().SkipWhile(l => l != "nearby tickets:").Skip(1))
        {
            yield return line.Split(",").Select(long.Parse).ToArray();
        }
    }

    private static long ErrorRate(IEnumerable<Rule> rules, long[] ticket)
    {
        var sum = 0L;
        foreach (var val in ticket)
        {
            if (!rules.Any(r => r.Valid(val)))
                sum += val;
        }
        return sum;
    }

    private static long GetErrorRate(string file)
    {
        var rules = GetRules(file).ToList();
        return OtherTickets(file).Sum(v => ErrorRate(rules, v));
    }

    protected override long? Part1()
    {
        Assert(GetErrorRate(Sample()), 71);
        return GetErrorRate(Input);
    }

    protected override long? Part2()
    {
        var rules = GetRules(Input).ToList();
        var validTickets = OtherTickets(Input).Where(t => ErrorRate(rules, t) == 0).ToList();

        var validColumns = rules.ToDictionary(
            r => r.Name,
            r => new HashSet<int>(
                Enumerable.Range(0, rules.Count)
                    .Where(col => !validTickets.Any(t => !r.Valid(t[col])))
            )
        );

        var columns = new Dictionary<string, int>();

        foreach (var col in validColumns.OrderBy(kvp => kvp.Value.Count))
            columns[col.Key] = col.Value.Except(columns.Values).Single();

        var myTicket = Input.Lines()
            .SkipWhile(l => l != "your ticket:").Skip(1)
            .First().Split(",").Select(long.Parse).ToArray();

        return columns
            .Where(kvp => kvp.Key.StartsWith("departure"))
            .Select(kvp => myTicket[kvp.Value])
            .Aggregate((a, b) => a * b);
    }
}
