namespace AdventOfCode._2024;

class Day11 : Solution
{
    private long Solve(string input, int times = 25)
    {
        var simulate = 
            Memoization.Recursive<long, int, long>((num, times, eval) => (num, times) switch
            {
                (_, 0) => 1,
                (0, _) => eval(1, times - 1),
                _ when (1 + (int)Math.Log10(num)) is {} digits 
                    && (digits % 2 == 0) 
                    && (int)Math.Pow(10, digits / 2) is {} divisor
                  => eval(num % divisor, times-1) + eval(num / divisor, times-1),
                _ => eval(num * 2024, times-1),
            });

        return Parser.Long.Token().Many().MustParse(input).Sum(n => simulate(n, times));
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample()), 55312);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        return Solve(Input,75);
    }
}
