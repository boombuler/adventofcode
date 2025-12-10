namespace AdventOfCode._2025;

using static Parser;

class Day10 : Solution
{
    record Machine(int[] Lights, int[][] Buttons, int[] Joltages);

    private static readonly Parser<Machine[]> InputParser = (
        from l in "[" + AnyChar(".#", [0, 1]).Many() + "]"
        from btn in ("(" + Int.List(',') + ")").Token().Many()
        from j in "{" + Int.List(',') + "}"
        select new Machine(l, btn, j)).List('\n');

    private long Solve(string input) => (
        from m in InputParser.MustParse(input)
        let result = m.Lights.Reverse().Aggregate(0L, (a, b) => (a << 1) + b)
        let buttons = m.Buttons.Select(btn => btn.Aggregate(0L, (a, b) => a | (1L << b))).ToList()
        select (
            from count in Enumerable.Range(1, buttons.Count - 1)
            where buttons.Combinations(count).Any(c => c.Aggregate((a, b) => a ^ b) == result)
            select count
        ).First()
    ).Sum();

    protected override long? Part1()
    {
        Assert(Solve(Sample()), 7);
        return Solve(Input);
    }
}
