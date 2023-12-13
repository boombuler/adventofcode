namespace AdventOfCode._2023;

class Day13 : Solution
{
    private static long Solve(string input, int smudges)
    {
        int GetMirror(int dim1, int dim2, Func<int, int, char> getPt) => (
            from mirrorIdx in Enumerable.Range(1, dim1 - 1)
            where smudges == (
                from scanIdx in Enumerable.Range(0, mirrorIdx)
                let flippedIdx = 2 * mirrorIdx - scanIdx - 1
                where flippedIdx >= 0 && flippedIdx < dim1
                select Enumerable.Range(0, dim2).Count(n => getPt(scanIdx, n) != getPt(flippedIdx, n))
            ).Sum()
            select mirrorIdx).FirstOrDefault();

        return (
            from patternStr in input.Split("\n\n")
            let pattern = patternStr.AsMap()
            let col = GetMirror(pattern.Width, pattern.Height, (a, b) => pattern[(a, b)])
            let row = col > 0 ? 0 : GetMirror(pattern.Height, pattern.Width, (a, b) => pattern[(b, a)])
            select (row * 100) + col
        ).Sum();
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample(), 0), 405);
        return Solve(Input, 0);
    }

    protected override long? Part2()
    {
        Assert(Solve(Sample(), 1), 400);
        return Solve(Input, 1);
    }
}
