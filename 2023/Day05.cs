namespace AdventOfCode._2023;

using static Parser;

class Day05 : Solution
{
    record Map((Range<long> SourceRange, long Offset)[] Mappings)
    {
        public Range<long>[] Apply(IEnumerable<Range<long>> ranges)
        {
            var mapped = new List<Range<long>>();

            foreach (var (srcRange, offset) in Mappings)
            {
                var open = new List<Range<long>>();

                foreach (var range in ranges)
                {
                    if (range.Intersect(srcRange, out var unmatched) is Range<long> i && i.Size > 0)
                        mapped.Add(i with { Start = i.Start + offset });
                    open.AddRange(unmatched);
                }
                ranges = open;
            }
            return [.. mapped, .. ranges];
        }
    }

    private static long FindLowestLocationNumber(string input, Parser<Range<long>[]> seedParser)
        => (from seed in "seeds: " + seedParser + "\n"
            from maps in (
                from _ in Any.Until(NL)
                from range in (
                    from d in Long.Token()
                    from s in Long.Token()
                    from l in Long.Token()
                    select (new Range<long>(s, l), d - s)
                ).List('\n')
                select new Map(range)
            ).Many()
            select maps.Aggregate(seed, (s, m) => m.Apply(s)).Min(r => r.Start)
        ).MustParse(input);

    protected override long? Part1()
    {
        static long Solve(string input)
            => FindLowestLocationNumber(input, Long.Token().Select(n => new Range<long>(n, 1)).Many());

        Assert(Solve(Sample()), 35);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input)
            => FindLowestLocationNumber(input, 
                 (
                    from s in Long + " "
                    from l in Long
                    select new Range<long>(s, l)
                ).Token().Many()
            );

        Assert(Solve(Sample()), 46);
        return Solve(Input);
    }
}