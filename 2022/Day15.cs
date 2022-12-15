namespace AdventOfCode._2022;

using System;

class Day15 : Solution
{
    record Range(long Min, long Max);

    record Sensor(Point2D Location, Point2D Beacon)
    {
        public Range ScanOnLine(long y)
        {
            var dx = Location.ManhattanDistance(Beacon) - Math.Abs(Location.Y - y);
            return (dx < 0) ? null : new Range(Location.X - dx, Location.X + dx);
        }
    }

    private IEnumerable<Sensor> Sensors(string input)
        => from line in input.Lines()
           let parts = line.Split('=', ',', ':')
           select new Sensor(
               new Point2D(long.Parse(parts[1]), long.Parse(parts[3])), 
               new Point2D(long.Parse(parts[5]), long.Parse(parts[7]))
           );

    private IEnumerable<Range> Ranges(IEnumerable<Sensor> Sensors, int y)
        => Sensors.Select(s => s.ScanOnLine(y)).Where(r => r != null).OrderBy(r => r.Min)
            .Aggregate(ImmutableStack<Range>.Empty,
                (s, r) => s.IsEmpty || (s.Peek().Max < r.Min) ? s.Push(r) : 
                    s.Pop().Push(new Range(s.Peek().Min, Math.Max(s.Peek().Max, r.Max))))
            .Reverse();
    
    private long CountScannedPoints(string input, int y) 
        => Ranges(Sensors(input), y).Sum(r => r.Max-r.Min);

    private long FindFreq(string input, int max)
    {
        var sens = Sensors(input).ToList();
        
        for (int y = 0; y <= max; y++)
        {
            long x = 0;
            foreach(var r in Ranges(sens, y))
            {
                if (r.Min > x)
                    return (x * 4000000) + y;
                if (x < r.Max)
                    x = r.Max + 1;
                if (x > max)
                    break;
            }
        }
        throw new InvalidDataException("No Solution!");
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
