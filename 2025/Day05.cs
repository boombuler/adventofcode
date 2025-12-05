namespace AdventOfCode._2025;

using static Parser;

class Day05 : Solution
{
    private static IEnumerable<Range<long>> GetRanges(string input)
        => (from s in Long + "-" from e in Long select new Range<long>(s, e - s + 1))
            .List('\n')
            .MustParse(input)
            .OrderBy(r => r.Start)
            .Aggregate(ImmutableList<Range<long>>.Empty, (result, current) => {
                if (result.Count > 0 && result[^1].TryMerge(current, out var merged))
                    return result.SetItem(result.Count - 1, merged);
                else
                    return result.Add(current);
            });

    protected override long? Part1()
    {
        static long Solve(string input)
        {
            var ids = input.Split("\n\n")[1].Lines().Select(long.Parse);
            var ranges = GetRanges(input).ToList();
            return ids.Count(i => ranges.TakeWhile(r => r.Start <= i).Any(r => r.Contains(i)));
        }

        Assert(Solve(Sample()), 3);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input) => GetRanges(input).Sum(r => r.Size);

        Assert(Solve(Sample()), 14);
        return Solve(Input);
    }
}
