namespace AdventOfCode._2023;

using System.Buffers;
using System.CommandLine;

class Day03 : Solution
{
    private static readonly Point2D Left = (-1, 0);
    private static readonly Point2D Right = (1, 0);
    private static IEnumerable<IGrouping<Point2D, long>> NumbersNearParts(string drawing, Predicate<char> partFilter)
    {
        var map = drawing.Cells().ToFrozenDictionary();

        bool IsDigit(Point2D pt)
            => map.TryGetValue(pt, out var digit) && char.IsAsciiDigit(digit);

        IEnumerable<Point2D> WalkDigits(Point2D pt, Point2D direction)
            => pt.Unfold(x => x + direction).Prepend(pt).TakeWhile(IsDigit);

        long ReadNumber(Point2D pt)
            => WalkDigits(WalkDigits(pt, Left).Last(), Right)
                .Select(p => map[p] - '0')
                .Aggregate((a, b) => (a * 10) + b);

        return (
            from kvp in map
            where partFilter(kvp.Value)
            from n in kvp.Key.Neighbours(withDiagonal: true)
            where IsDigit(n)
            select (Gear: kvp.Key, Number: ReadNumber(n))
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
