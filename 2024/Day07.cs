namespace AdventOfCode._2024;

using static Parser;

class Day07 : Solution
{
    private bool CanBeValid(long requiredTotal, long[] operands, Func<long,long,long>[] operations)
    {
        var open = new Stack<(long Total, Func<long,long,long> Operation, int Index)>();
        open.Push((0, (_, b) => b, 0));
        while (open.TryPop(out var cur))
        {
            if (cur.Index == operands.Length)
            {
                if (cur.Total == requiredTotal)
                    return true;
                continue;
            }
            foreach (var op in operations)
            {
                var total = op(cur.Total, operands[cur.Index]);
                if (total <= requiredTotal)
                    open.Push((total, op, cur.Index + 1));
            }
        }
        return false;
    }

    private long Solve(string input, bool withConcat)
    {
        var operations = new[] { (long a, long b) => a+b, (a,b) => a*b };
        if (withConcat)
            operations = [..operations, MathExt.Concat];
        
        return input.Lines().Select((Long + ": ").Then(Long.Token().Many()).MustParse)
            .AsParallel().AsUnordered()
            .Where(n => CanBeValid(n.Item1, n.Item2, operations)).Sum(n => n.Item1);
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample(), false), 3749);
        return Solve(Input, false);
    }

    protected override long? Part2()
    {
        Assert(Solve(Sample(), true), 11387);
        return Solve(Input, true);
    }
}
