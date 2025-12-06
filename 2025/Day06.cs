namespace AdventOfCode._2025;

class Day06 : Solution
{
    private long SolveMathSheet(string input, Func<Point2D<int>, int> groupSelector)
    {
        var match = new Regex("^(?<numbers>.*)\n(?<op>[+*] +)+$", RegexOptions.Singleline).Match(input);
        var numberMap = new StringMap<int>(match.Groups["numbers"].Value, c => c == ' ' ? -1 : c - '0');

        return match.Groups["op"].Captures.Scan((i: 0, Result: 0L), (l, op) => (l.i + op.Length, 
        (
            from row in Enumerable.Range(0, numberMap.Size.Y)    // Iterate all digits in the rows above the operator
            from col in Enumerable.Range(l.i, op.Length)
            let pt = new Point2D<int>(col, row)
            let d = numberMap[pt]
            where d > 0                                          // Skip whitespaces in the number table
            group d by groupSelector(pt) into digits             // Group them either by row or by column
            select digits.Aggregate(0L, (n, d) => (n * 10) + d)  // Convert digits to long-values
        ).Aggregate(op.Value[0].ToBinaryOperator<long>()))).Sum(l => l.Result);
    }

    protected override long? Part1()
    {
        long Solve(string input) => SolveMathSheet(input, pt => pt.Y);

        Assert(Solve(Sample()), 4277556);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input) => SolveMathSheet(input, pt => pt.X);

        Assert(Solve(Sample()), 3263827);
        return Solve(Input);
    }
}
