namespace AdventOfCode._2023;

using static Parser;

class Day05 : Solution
{
    record struct Range(long Start, long Size)
    {
        public long End => Start + Size;

        public Range? Intersect(Range other, out IEnumerable<Range> unintersected)
        {
            var olStart = Math.Max(Start, other.Start);
            var olEnd = Math.Min(End, other.End);
            if (olEnd < olStart)
            {
                unintersected = [this];
                return null;
            }
            unintersected = new[] {
                new Range(Start, olStart - Start),
                new Range(olEnd, End - olEnd)
            }.Where(i => i.Size > 0);
            return new Range(olStart, olEnd - olStart);
        }
    }

    record Map((Range SourceRange, long Offset)[] Mappings)
    {
        public Range[] Apply(IEnumerable<Range> ranges)
        {
            var mapped = new List<Range>();

            foreach (var (srcRange, offset) in Mappings)
            {
                var open = new List<Range>();

                foreach (var range in ranges)
                {
                    if (range.Intersect(srcRange, out var unmatched) is Range i)
                        mapped.Add(i with { Start = i.Start + offset });
                    open.AddRange(unmatched);
                }
                ranges = open;
            }
            return mapped.Concat(ranges).ToArray();
        }
    }

    private static long FindLowestLocationNumber(string input, Parser<Range[]> seedParser)
        => (from seed in "seeds: " + seedParser + "\n"
            from maps in (
                from _ in Any.Until(NL)
                from range in (
                    from d in Int.Token()
                    from s in Int.Token()
                    from l in Int.Token()
                    select (new Range(s, l), d - s)
                ).List('\n')
                select new Map(range)
            ).Many()
            select maps.Aggregate(seed, (s, m) => m.Apply(s)).Min(r => r.Start)
        ).MustParse(input);

    protected override long? Part1()
    {
        static long Solve(string input)
            => FindLowestLocationNumber(input, Int.Token().Select(n => new Range(n, 1)).Many());

        Assert(Solve(Sample()), 35);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input)
            => FindLowestLocationNumber(input, 
                 (
                    from s in Int + " "
                    from l in Int
                    select new Range(s, l)
                ).Token().Many()
            );

        Assert(Solve(Sample()), 46);
        return Solve(Input);
    }
}