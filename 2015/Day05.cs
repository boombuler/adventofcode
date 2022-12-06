namespace AdventOfCode._2015;

class Day05 : Solution
{
    private static readonly Regex Rule1 = new(@"(.*[aeiou]){3}", RegexOptions.Compiled);
    private static readonly Regex Rule2 = new(@"(\w)\1", RegexOptions.Compiled);
    private static readonly Regex Rule3 = new(@"^((?!(ab|cd|pq|xy)).)*$", RegexOptions.Compiled);

    private static readonly Regex Rule4 = new(@"(\w\w).*\1", RegexOptions.Compiled);
    private static readonly Regex Rule5 = new(@"(\w)\w\1", RegexOptions.Compiled);

    private bool IsNice(string input)
        => Rule1.IsMatch(input) && Rule2.IsMatch(input) && Rule3.IsMatch(input);
    private bool IsNice2(string input)
        => Rule4.IsMatch(input) && Rule5.IsMatch(input);

    protected override long? Part1()
    {
        Assert(IsNice("ugknbfddgicrmopn"));
        Assert(IsNice("aaa"));
        Assert(!IsNice("jchzalrnumimnmhp"));
        Assert(!IsNice("haegwjzuvuyypxyu"));
        Assert(!IsNice("dvszwmarrgswjxmb"));
        return Input.Lines().Where(IsNice).Count();
    }
    protected override long? Part2()
    {
        Assert(IsNice2("xyxy"));
        Assert(!IsNice2("aaa"));
        Assert(IsNice2("qjhvhtzxzqqjkmpb"));
        Assert(IsNice2("xxyxx"));
        Assert(!IsNice2("uurcxstgmygtbstg"));
        Assert(!IsNice2("ieodomkazucvgmuy"));
        return Input.Lines().Where(IsNice2).Count();
    }
}
