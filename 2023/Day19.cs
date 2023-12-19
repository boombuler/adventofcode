namespace AdventOfCode._2023;

using static Parser;
using Range = Range<long>;

class Day19 : Solution
{
    const string Dimensions = "xmas";
    record Condition(int Idx, Range Range, string Target);
    record Workflow(string Name, Condition[] Conditions, string Fallback);

    private static readonly Parser<Condition> ConditionParser = (
        from propIdx in AnyChar(Dimensions).Select(Dimensions.IndexOf)
        from comp in AnyChar("<>")
        from val in Int + ":"
        from target in Word
        let range = comp == '<' ? new Range(1, val - 1) : new Range(val + 1, long.MaxValue - val - 1)
        select new Condition(propIdx, range, target)
    );

    private static readonly Parser<Workflow[]> WorkflowParser = (
        from name in Word + "{"
        from conditions in ConditionParser.List(',')
        from fall in "," + Word + "}"
        select new Workflow(name, conditions, fall)
    ).List('\n');

    private static IEnumerable<Range[]> GetValidRanges(string wfStr)
    {
        var workflows = WorkflowParser.MustParse(wfStr).ToFrozenDictionary(w => w.Name);
        var queue = new Queue<(string WorkflowName, Range[] Ranges)>();
        queue.Enqueue(("in", Enumerable.Repeat(new Range(1, long.MaxValue-1), Dimensions.Length).ToArray()));
        while (queue.TryDequeue(out var cur))
        {
            switch (cur.WorkflowName)
            {
                case "R": 
                    break;
                case "A": 
                    yield return cur.Ranges; 
                    break;
                default:
                    var wf = workflows[cur.WorkflowName];
                    var r = cur.Ranges;
                    foreach (var cond in wf.Conditions)
                    {
                        var newRanges = (Range[])r.Clone();
                        newRanges[cond.Idx] = r[cond.Idx].Intersect(cond.Range, out var remaining);
                        r[cond.Idx] = remaining.SingleOrDefault(new Range(0, 0));
                        queue.Enqueue((cond.Target, newRanges));
                    }
                    queue.Enqueue((wf.Fallback, r));
                    break;
            }
        }
    }

    protected override long? Part1()
    {
        static long Solve(string input)
        {
            var (wfStr, (pStr, _)) = input.Split("\n\n");
            var parts = ("{" + ((Any + "=").ThenR(Int)).List(',') + "}").List('\n').MustParse(pStr);

            return (
                from range in GetValidRanges(wfStr)
                from p in parts
                where range.Zip(p).All(n => n.First.Contains(n.Second))
                select p.Sum()
            ).Sum();
        }

        Assert(Solve(Sample()), 19114);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input)
            => GetValidRanges(input.Split("\n\n").First())
                .Select(r => 
                    r.Zip(Enumerable.Repeat(new Range(1, 4000), Dimensions.Length), 
                    (a, b) => a.Intersect(b, out _).Size))
                .Sum(values => values.Aggregate((a,b) => a*b));

        Assert(Solve(Sample()), 167409079868000);
        return Solve(Input);
    }
}
