namespace AdventOfCode._2017;

class Day02 : Solution
{
    private static long CalcChecksum(string input)
        => input.Lines().Select(l =>
        {
            var (min, max) = l.Split('\t', ' ').Select(int.Parse).MinMax();
            return max - min;
        }).Sum();

    protected override long? Part1()
    {
        Assert(CalcChecksum(Sample("1")), 18);
        return CalcChecksum(Input);
    }

    private static long SumEvenlyDivisibleValues(string input)
    {
        var sum = 0L;
        foreach (var line in input.Lines().Select(l => l.Split('\t', ' ').Select(int.Parse).ToArray()))
        {
            foreach (var (a, b) in line.CombinationPairs())
            {
                if (a == b) 
                    continue;
                if (a > 0 && b % a == 0)
                    sum += b / a;
                if (b > 0 && a % b == 0)
                    sum += a / b;
            }
        }
        return sum;
    }

    protected override long? Part2()
    {
        Assert(SumEvenlyDivisibleValues(Sample("2")), 9);
        return SumEvenlyDivisibleValues(Input);
    }
}
