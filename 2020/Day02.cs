namespace AdventOfCode._2020;

class Day02 : Solution
{
    record PasswordLine(int I1, int I2, char C, string Test);
    private static readonly Func<string, PasswordLine?> ParsePasswordLine =
        new Regex(@"(?<I1>\d+)\-(?<I2>\d+)\W(?<C>\w):\W(?<Test>\w+)", RegexOptions.Compiled).ToFactory<PasswordLine>();

    static bool IsPasswordValidPart1(string line)
    {
        (int lowerBound, int upperBound, char testChar, string test) = ParsePasswordLine(line) ?? throw new InvalidInputException();
        var testCharCount = test.Where(c => c == testChar).Count();
        return (testCharCount >= lowerBound) && (testCharCount <= upperBound);
    }

    static bool IsPasswordValidPart2(string line)
    {
        (int i1, int i2, char c, string test) = ParsePasswordLine(line) ?? throw new InvalidInputException();
        bool checkIndex(int i) => test[i - 1] == c;
        return checkIndex(i1) ^ checkIndex(i2);
    }
    protected override long? Part1() => Input.Lines().Where(IsPasswordValidPart1).Count();
    protected override long? Part2() => Input.Lines().Where(IsPasswordValidPart2).Count();
}
