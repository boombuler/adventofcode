namespace AdventOfCode._2015;

class Day02 : Solution
{
    protected override long? Part1()
    {
        static long GetPaperSize(string dim)
        {
            var (total, smallest) = dim.Split('x').Select(int.Parse)
                .CombinationPairs((a, b) => a * b)
                .Aggregate((Total: 0, Smallest: int.MaxValue), (a, d) => (a.Total + 2 * d, d < a.Smallest ? d : a.Smallest));
            return total + smallest;
        }

        Assert(GetPaperSize("2x3x4"), 58);
        Assert(GetPaperSize("1x1x10"), 43);
        return Input.Lines().Sum(GetPaperSize);
    }
    protected override long? Part2()
    {
        static long GetRibbonLength(string dim)
        {
            var items = dim.Split('x').Select(int.Parse).ToArray();
            return items.Order().Take(2).Sum(s => s * 2)
                + items.Aggregate((a, b) => a * b);
        }

        Assert(GetRibbonLength("2x3x4"), 34);
        Assert(GetRibbonLength("1x1x10"), 14);
        return Input.Lines().Sum(GetRibbonLength);
    }
}
