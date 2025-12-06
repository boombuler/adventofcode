namespace AdventOfCode;

interface ISolution
{
    int Year { get; }
    int Day { get; }

    Task<string> Part1(IOutput? output);
    Task<string> Part2(IOutput? output);
}
