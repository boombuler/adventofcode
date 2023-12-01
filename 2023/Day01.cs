namespace AdventOfCode._2023;

class Day01 : Solution
{
    private readonly string[] Digits = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
    private readonly string[] Words = ["-", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

    private int GetDigits(string l, params string[][] alphabets)
    {
        IEnumerable<int> GetIndices(Func<string, int> indexOf)
            => alphabets.SelectMany(a => a.Select((word, i) => new { Index = indexOf(word), Digit = i }))
                .Where(n => n.Index >= 0).OrderBy(n => n.Index).Select(n => n.Digit);

        return GetIndices(l.IndexOf).First() * 10 +
            GetIndices(l.LastIndexOf).Last();
    }

    protected override long? Part1()
        => Input.Lines().Sum(l => GetDigits(l, Digits));

    protected override long? Part2()
        => Input.Lines().Sum(l => GetDigits(l, Digits, Words));
}
