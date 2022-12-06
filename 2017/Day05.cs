namespace AdventOfCode._2017;

class Day05 : Solution
{
    private static int CountJumps(string input, int upperBound = int.MaxValue)
    {
        var lines = input.Lines().Select(int.Parse).ToList();
        int pc = 0;
        int result = 0;

        while (pc >= 0 && pc < lines.Count)
        {
            var value = lines[pc];
            lines[pc] += (value >= upperBound) ? -1 : 1;
            pc += value;
            result++;
        }

        return result;
    }

    protected override long? Part1()
    {
        Assert(CountJumps(Sample()), 5);
        return CountJumps(Input);
    }

    protected override long? Part2()
    {
        Assert(CountJumps(Sample(), 3), 10);
        return CountJumps(Input, 3);
    }

}
