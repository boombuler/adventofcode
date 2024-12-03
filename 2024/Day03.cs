namespace AdventOfCode._2024;

class Day03 : Solution
{
    private const string SamplePart1 = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
    private const string SamplePart2 = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";

    record State(bool Enabled, long Sum);

    private long Solve(string input, bool enableDoAndDont)
    {
        var parser = 
            from _ in Parser.Str("mul(")
            from l in Parser.Long + ","
            from r in Parser.Long + ")"
            select new Func<State, State>(s => s.Enabled ? s with { Sum = s.Sum + l * r } : s);

        if (enableDoAndDont)
        {
            parser |= Parser.Str("do()", new Func<State, State>(s => s with { Enabled = true }));
            parser |= Parser.Str("don't()", new Func<State, State>(s => s with { Enabled = false }));
        }
        
        parser |= Parser.Any.Return<Func<State, State>>(s => s); // Skip Junk
        return parser.Many().MustParse(input).Aggregate(new State(true, 0), (s, f) => f(s)).Sum;
    }

    protected override long? Part1()
    {
        Assert(Solve(SamplePart1, false), 161);
        return Solve(Input, false);
    }

    protected override long? Part2()
    {
        Assert(Solve(SamplePart2, true), 48);
        return Solve(Input, true);
    }
}