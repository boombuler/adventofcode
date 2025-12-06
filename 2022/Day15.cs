namespace AdventOfCode._2022;

using Point = Point2D<int>;

class Day15 : Solution
{
    record Range(long Min, long Max);

    record Sensor(Point Location, Point Beacon)
    {
        public Range? ScanOnLine(long y)
        {
            var dx = Location.ManhattanDistance(Beacon) - Math.Abs(Location.Y - y);
            return (dx < 0) ? null : new Range(Location.X - dx, Location.X + dx);
        }
    }

    private static IEnumerable<Sensor> Sensors(string input)
        => from line in input.Lines()
           let parts = line.Split('=', ',', ':')
           select new Sensor(
               new Point(int.Parse(parts[1]), int.Parse(parts[3])),
               new Point(int.Parse(parts[5]), int.Parse(parts[7]))
           );

    private static IEnumerable<Range> Ranges(IEnumerable<Sensor> Sensors, int y)
        => Sensors.Select(s => s.ScanOnLine(y)).NonNull().OrderBy(r => r.Min)
            .Aggregate(ImmutableStack<Range>.Empty,
                (s, r) => s.IsEmpty || (s.Peek().Max < r.Min) ? s.Push(r) :
                    s.Pop().Push(new Range(s.Peek().Min, Math.Max(s.Peek().Max, r.Max))));

    private static long CountScannedPoints(string input, int y)
        => Ranges(Sensors(input), y).Sum(r => r.Max - r.Min);

    private static long? FindFreq(string input, int max)
    {
        var sens = Sensors(input).ToList();

        return Enumerable.Range(0, max + 1)
            .Select(y => y + (4000000 * (
                Ranges(sens, y)
                    .Where(r => r.Min <= max && r.Max >= 0)
                    .OrderBy(r => r.Min)
                    .Skip(1).FirstOrDefault()?.Min - 1))
            ).First(r => r.HasValue);
    }

    protected override long? Part1()
    {
        Assert(CountScannedPoints(Sample(), 10), 26);
        return CountScannedPoints(Input, 2000000);
    }

    protected override long? Part2()
    {
        Assert(FindFreq(Sample(), 20), 56000011);
        return FindFreq(Input, 4000000);
    }
}
