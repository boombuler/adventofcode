namespace AdventOfCode._2024;

class Day19 : Solution
{
    private long CheckPattern(ILookup<char, string> towels, ReadOnlySpan<char> pattern)
    {
        var open = new PriorityQueue<int, int>([(0, 0)]);
        var possibilities = new Dictionary<int, long>() { [0] = 1 };

        while (open.TryDequeue(out var index, out _))
        {
            if (index == pattern.Length)
                continue;

            var remainingPattern = pattern.Slice(index);

            foreach (var towel in towels[pattern[index]])
            {
                if (!remainingPattern.StartsWith(towel))
                    continue;

                var newIndex = index + towel.Length;
                if (!possibilities.ContainsKey(newIndex))
                    open.Enqueue(newIndex, newIndex);
                possibilities[newIndex] = possibilities.GetValueOrDefault(newIndex) + possibilities[index];
            }
        }

        return possibilities.GetValueOrDefault(pattern.Length);
    }

    private ParallelQuery<long> GetPossiblePatternCounts(string input)
    {
        var (towelsStr, (patternsStr, _)) = input.Split("\n\n");
        var towels = towelsStr.Split(", ").ToLookup(t => t[0]);
        var patterns = patternsStr.Split("\n");
        return patterns.AsParallel().AsUnordered().Select(p => CheckPattern(towels, p));
    }

    protected override long? Part1()
    {
        long Solve(string input)
            => GetPossiblePatternCounts(input).Count(n => n > 0);

        Assert(Solve(Sample()), 6);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input)
            => GetPossiblePatternCounts(input).Sum();

        Assert(Solve(Sample()), 16);
        return Solve(Input);
    }
}
