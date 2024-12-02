namespace AdventOfCode._2024;

class Day02 : Solution
{
    private int BadIndex(IEnumerable<int> numbers, int? skip = null) 
        => numbers.Where((_, i) => i != skip)
            .Pairwise((a, b) => a-b).Select(x => new { Sign = Math.Sign(x), Abs = Math.Abs(x) })
            .Scan((Sign:(int?)null, Bad: false), (aggr, x) => (x.Sign, (x.Abs is < 1 or > 3) || (x.Sign != (aggr.Sign ?? x.Sign))))
            .Skip(1)
            .Select((x, i) => new { x.Bad, Idx = i })
            .FirstOrDefault(x => x.Bad)?.Idx ?? -1;

    private int CountRoutes(string input, Func<int[], bool> predicate)
        => input.Lines().Select(Parser.Int.Token().Many().MustParse).Count(predicate);
    
    protected override long? Part1()
    {
        int Solve(string input) => CountRoutes(input, x => BadIndex(x) < 0);

        Assert(Solve(Sample()), 2);
        return Solve(Input);
    }
    
    protected override long? Part2()
    {
        int Solve(string input) 
            => CountRoutes(input, line => BadIndex(line) is int idx && 
                (idx < 0 || BadIndex(line, skip: idx) < 0 || BadIndex(line, skip: idx + 1) < 0));

        Assert(Solve(Sample()), 4);
        return Solve(Input);
    }
}
