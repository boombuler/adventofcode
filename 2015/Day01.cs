namespace AdventOfCode._2015;
class Day01 : Solution
{
    public static IEnumerable<int> WalkLevels(string input)
        => input.Select(c => c == '(' ? 1 : -1).Scan(0, (a, b) => a + b);

    protected override long? Part1() 
        => WalkLevels(Input).Last();

    protected override long? Part2()
        => WalkLevels(Input).TakeWhile(n => n >= 0).Count();
}
