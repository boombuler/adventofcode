namespace AdventOfCode._2023;

class Day09 : Solution
{
    private static int SumNextValues(string input, Func<int[], int, int> combine)
    {
        int Extrapolate(int[] data)
        {
            var next = data.SlidingWindow(2).Select(n => n[1] - n[0]).ToArray();
            if (next.Any(n => n != 0))
                return combine(data, Extrapolate(next));
            return data[0];
        }

        return input.Lines().Select(l => l.Split(' ').Select(int.Parse).ToArray()).Sum(Extrapolate);
    }

    protected override long? Part1()
    {
        static long Solve(string input)
            => SumNextValues(input, (data, x) => data.Last() + x);

        Assert(Solve(Sample()), 114);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input)
            => SumNextValues(input, (data, x) => data.First() - x);

        Assert(Solve(Sample()), 2);
        return Solve(Input);
    }
}
