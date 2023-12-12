namespace AdventOfCode._2023;

using Point = Point2D<int>;

class Day03 : Solution
{
    private static IEnumerable<IGrouping<Point, long>> NumbersNearParts(string drawing, Predicate<char> partFilter)
    {
        var map = drawing.AsMap();

        bool IsDigit(Point pt)
            => map.TryGetValue(pt, out var digit) && char.IsAsciiDigit(digit);

        IEnumerable<Point> WalkDigits(Point pt, Point direction)
            => pt.Unfold(x => x + direction).Prepend(pt).TakeWhile(IsDigit);

        long ReadNumber(Point pt)
            => WalkDigits(WalkDigits(pt, Point.Left).Last(), Point.Right)
                .Select(map.GetValueOrDefault)
                .Aggregate(0L, MathExt.AppendDigit);

        return (
            from kvp in map
            where partFilter(kvp.Value)
            from n in kvp.Index.Neighbours(withDiagonal: true)
            where IsDigit(n)
            select (Gear: kvp.Index, Number: ReadNumber(n))
        ).Distinct().GroupBy(n => n.Gear, n => n.Number);
    }

    protected override long? Part1()
    {
        static long SumPartNumbers(string input)
            => NumbersNearParts(input, p => !char.IsAsciiDigit(p) && p != '.')
                .SelectMany(g => g)
                .Sum();

        Assert(SumPartNumbers(Sample()), 4361);
        return SumPartNumbers(Input);
    }

    protected override long? Part2()
    {
        static long SumGearRatio(string input)
            => NumbersNearParts(input, p => p == '*')
                .Where(g => g.Count() == 2)
                .Sum(g => g.Aggregate((a, b) => a * b));

        Assert(SumGearRatio(Sample()), 467835);
        return SumGearRatio(Input);
    }
}
