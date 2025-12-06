namespace AdventOfCode._2018;

class Day03 : Solution
{
    record Claim(int ID, int Top, int Left, int Width, int Height)
    {
        public int Right => Left + Width - 1;
        public int Bottom => Top + Height - 1;
        public IEnumerable<Point2D<int>> Squares() => Point2D<int>.Range((Left, Top), (Right, Bottom));
        public bool Overlaps(Claim o) => Right >= o.Left && o.Right >= Left && Bottom >= o.Top && o.Bottom >= Top;
    }
    private static readonly Func<string, Claim?> ParseClaim = new Regex(@"#(?<ID>\d+) @ (?<Left>\d+),(?<Top>\d+): (?<Width>\d+)x(?<Height>\d+)", RegexOptions.Compiled).ToFactory<Claim>();

    private static long GetOverlapping(string input)
        => input.Lines().Select(ParseClaim).NonNull().SelectMany(c => c.Squares()).GroupBy(s => s).Count(g => g.Count() > 1);

    private static int GetNonOverlappingClaimId(string input)
    {
        var claims = input.Lines().Select(ParseClaim).NonNull().ToList();
        return claims.First(claim => !claims.Any(other => claim != other && other.Overlaps(claim))).ID;
    }

    protected override long? Part1()
    {
        Assert(GetOverlapping(Sample()), 4);
        return GetOverlapping(Input);
    }

    protected override long? Part2()
    {
        Assert(GetNonOverlappingClaimId(Sample()), 3);
        return GetNonOverlappingClaimId(Input);
    }

}
