namespace AdventOfCode._2015;

class Day17 : Solution
{
    private static IEnumerable<int> GetPossibilities(string data, long liters)
    {
        static IEnumerable<int> count(IEnumerable<long> containers, long rest, int usedItems)
        {
            if (!containers.Any())
                return Enumerable.Empty<int>();
            var i = containers.First();
            var others = containers.Skip(1);
            var rem = rest - i;
            var result = count(others, rest, usedItems).ToList();
            if (rem == 0)
            {
                result.Add(usedItems + 1);
                return result;
            }
            if (rem < 0)
                return result;
            result.AddRange(count(others, rem, usedItems + 1));
            return result;
        }

        var items = data.Lines().Select(long.Parse).ToList();
        return count(items, liters, 0).Where(x => x > 0);
    }

    private static long FindMinimumPossibilities(string input, long liters)
    {
        var pos = GetPossibilities(input, liters).ToList();
        var minComb = pos.Min();
        return pos.Where(x => x == minComb).Count();
    }

    protected override long? Part1()
    {
        Assert(GetPossibilities(Sample(), 25).Count(), 4);
        return GetPossibilities(Input, 150).Count();
    }

    protected override long? Part2()
    {
        Assert(FindMinimumPossibilities(Sample(), 25), 3);
        return FindMinimumPossibilities(Input, 150);
    }
}
