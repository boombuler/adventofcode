namespace AdventOfCode._2017;

class Day15 : Solution
{
    private static (long A, long B) SAMPLE = (65, 8921);

    private static long Next(long current, long factor) => (current * factor) % 2147483647;

    private static long TestSamples((long A, long B) startingValues, int count, (long A, long B)? divisors = null)
    {
        var a = startingValues.A.Unfold(n => Next(n, 16807));
        var b = startingValues.B.Unfold(n => Next(n, 48271));
        if (divisors.HasValue)
        {
            a = a.Where(n => (n % divisors.Value.A) == 0);
            b = b.Where(n => (n % divisors.Value.B) == 0);
        }
        return a.Zip(b).Take(count).Count(v => ((v.First ^ v.Second) & 0xFFFF) == 0);
    }

    private (long A, long B) GeneratorValues
    {
        get
        {
            var vals = Input.Lines().Select(line => line[line.LastIndexOf(' ')..]).Select(long.Parse).ToArray();
            return (vals[0], vals[1]);
        }
    }

    protected override long? Part1()
    {
        const int COUNT = 40_000_000;
        Assert(TestSamples(SAMPLE, COUNT), 588);
        return TestSamples(GeneratorValues, COUNT);
    }

    protected override long? Part2()
    {
        const int COUNT = 5_000_000;
        (long A, long B) divisors = (4, 8);
        Assert(TestSamples(SAMPLE, COUNT, divisors), 309);
        return TestSamples(GeneratorValues, COUNT, divisors);
    }
}
