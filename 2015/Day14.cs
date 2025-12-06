namespace AdventOfCode._2015;

class Day14 : Solution
{
    record Reindeer(string Name, int Speed, int FlyTime, int RestTime)
    {
        // Dancer can fly 16 km/s for 11 seconds, but then must rest for 162 seconds.
        public static Func<string, Reindeer?> Parse =
            new Regex(@"(?<Name>\w+) can fly (?<Speed>\d+) km/s for (?<FlyTime>\d+) seconds, but then must rest for (?<RestTime>\d+) seconds\.").ToFactory<Reindeer>();

        public int Score { get; set; }

        public int CycleTime => FlyTime + RestTime;

        public long DistanceAfter(int seconds)
        {
            int flySeconds = (FlyTime * (seconds / CycleTime)) + Math.Min(FlyTime, seconds % CycleTime);
            return (flySeconds * Speed);
        }
    }

    private static long WinningDistance(string reindeerTxt, int seconds)
        => reindeerTxt.Lines().Select(Reindeer.Parse).NonNull().Select(rd => rd.DistanceAfter(seconds)).Max();

    protected override long? Part1()
    {
        Assert(WinningDistance(Sample(), 10), 160);
        Assert(WinningDistance(Sample(), 1000), 1120);
        return WinningDistance(Input, 2503);
    }

    private static long Race(string reindeerTxt, int rounds)
    {
        var reindeers = reindeerTxt.Lines().Select(Reindeer.Parse).NonNull().ToList();

        for (int i = 1; i <= rounds; i++)
        {
            var winners = reindeers.GroupBy(r => r.DistanceAfter(i)).OrderByDescending(grp => grp.Key).First();
            foreach (var w in winners)
                w.Score++;
        }
        return reindeers.Select(r => r.Score).Max();
    }

    protected override long? Part2()
    {
        Assert(Race(Sample(), 1000), 689);
        return Race(Input, 2503);
    }
}
