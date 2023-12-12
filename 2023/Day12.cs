namespace AdventOfCode._2023;

using static Parser;

class Day12 : Solution
{
    private static long CountPossibleSolutionsForLine(string inputStr, int unfold)
    {
        var (pattern, numbers) = (
            from c in AnyChar("?.#").Many().Token().Text()
            from n in Int.List(',')
            select (
                string.Join('?', Enumerable.Repeat(c, unfold)),
                Enumerable.Repeat(n, unfold).SelectMany(n => n).ToArray()
            )
        ).MustParse(inputStr); 

        var foundSolutions = new Dictionary<string, long>();
        long SolveRec(int numberIdx, string pattern)
        {
            if (pattern.Length == 0)
                return numberIdx == numbers.Length ? 1 : 0;
            var key = numberIdx + '|' + pattern;
            if (foundSolutions.TryGetValue(key, out var res))
                return res;

            long CalcValue()
            {
                switch (pattern[0])
                {
                    case '.':
                        return SolveRec(numberIdx, pattern[1..]);
                    case '?':
                        return SolveRec(numberIdx, "#" + pattern[1..]) + SolveRec(numberIdx, "." + pattern[1..]);
                    default: // #
                        if (numberIdx == numbers.Length)
                            return 0;
                        var n = numbers[numberIdx];
                        if (n > pattern.Length || pattern[0..n].Contains('.'))
                            return 0;

                        var remaining = pattern[n..];
                        if (remaining.Length == 0)
                            return SolveRec(numberIdx + 1, string.Empty);
                        if (remaining[0] == '#')
                            return 0;
                        return SolveRec(numberIdx+1, remaining[1..]);
                        
                }
            }
            return foundSolutions[key] = CalcValue();
        }
       
        return SolveRec(0, pattern);
    }

    private static long CountPossibleSolutions(string input, int unfold = 1)
        => input.Lines().Sum(l => CountPossibleSolutionsForLine(l, unfold));
    
    protected override long? Part1()
    {
        Assert(CountPossibleSolutions(Sample()), 21);
        return CountPossibleSolutions(Input);
    }

    protected override long? Part2()
    {
        Assert(CountPossibleSolutions(Sample(), 5), 525152);
        return CountPossibleSolutions(Input, 5);
    }
}
