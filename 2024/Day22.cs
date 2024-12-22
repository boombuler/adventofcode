namespace AdventOfCode._2024;

class Day22 : Solution
{
    long GetNextNum(long seed)
    {
        seed = (seed ^ (seed <<  6)) % 16777216;
        seed = (seed ^ (seed >>  5)) % 16777216;
        return (seed ^ (seed << 11)) % 16777216;
    }

    IEnumerable<IEnumerable<long>> Generators(string input)
        => input.Lines().Select(long.Parse).Select(seed => seed.Unfold(GetNextNum).Prepend(seed));

    protected override long? Part1()
    {
        long Solve(string input)
            => Generators(input).Select(gen => gen.Skip(2000).First()).Sum();

        Assert(Solve("1\r\n10\r\n100\r\n2024"), 37327623);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input)
            => Generators(input)
                .SelectMany(gen =>
                    gen.Select(n => (int)(n % 10)).Take(2000)
                    .Scan((Seq: 0, Value: 0), (last, digit) => (((last.Seq << 5) | (last.Value - digit + 9)) & 0x0FFFFF, digit))
                    .Skip(4).DistinctBy(wnd => wnd.Seq))
                .GroupBy(s => s.Seq, g => g.Value)
                .Max(grp => grp.Sum());

        Assert(Solve("1\r\n2\r\n3\r\n2024"), 23);
        return Solve(Input);
    }
}
