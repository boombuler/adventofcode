namespace AdventOfCode._2024;

using Clique = ImmutableHashSet<string>;

class Day23 : Solution<long?, string>
{
    private static IEnumerable<Clique> FindMaxCliques(string input)
    {
        var connections = input.Lines()
            .SelectMany(l => new (string, string)[] { (l[0..2], l[3..5]), (l[3..5], l[0..2]) })
            .ToLookup(t => t.Item1, t => t.Item2);
        var open = new Stack<(Clique current, Clique candidates, Clique exclude)>(
            [(Clique.Empty, connections.Select(n => n.Key).ToImmutableHashSet(), Clique.Empty)]
        );
        var connectionCounts = connections.ToDictionary(n => n.Key, n => n.Count());
        while (open.TryPop(out var cur))
        {
            var (current, candidates, exclude) = cur;
            if (candidates.IsEmpty && exclude.IsEmpty)
                yield return current;
            var pivot = candidates.MaxBy(v => connectionCounts[v]) ?? string.Empty;
            foreach (var v in candidates.Except(connections[pivot]))
            {
                open.Push((current.Add(v), candidates.Intersect(connections[v]), exclude.Intersect(connections[v])));
                candidates = candidates.Remove(v);
                exclude = exclude.Add(v);
            }
        }
    }

    protected override long? Part1()
    {
        static long Solve(string input) 
            => FindMaxCliques(input).SelectMany(c => c.Combinations(3))
                .Select(c => string.Join(",", c.Order())).Distinct()
                .Count(new Regex("^t|,t", RegexOptions.Compiled).IsMatch);
        
        Assert(Solve(Sample()), 7);
        return Solve(Input);
    }

    protected override string Part2()
    {
        static string Solve(string input) 
            => string.Join(",", FindMaxCliques(input).MaxBy(n => n.Count)!.Order());

        Assert(Solve(Sample()), "co,de,ka,ta");
        return Solve(Input);
    }
}
