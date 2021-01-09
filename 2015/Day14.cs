using AdventOfCode.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2015
{
    class Day14 : Solution
    {
        class Reindeer
        {
            // Dancer can fly 16 km/s for 11 seconds, but then must rest for 162 seconds.
            public static Func<string, Reindeer> Parse = 
                new Regex(@"(?<name>\w+) can fly (?<speed>\d+) km/s for (?<flytime>\d+) seconds, but then must rest for (?<resttime>\d+) seconds\.").ToFactory<Reindeer>();

            public string Name { get; }
            public int Speed { get; }
            public int FlyTime { get; }
            public int RestTime { get; }
            public int Score { get; set; }

            public int CycleTime => FlyTime + RestTime; 

            private Reindeer(string name, int speed, int flytime, int resttime)
            {
                Name = name;
                Speed = speed;
                FlyTime = flytime;
                RestTime = resttime;
            }

            public long DistanceAfter(int seconds)
            {
                int flySeconds = (FlyTime * (seconds / CycleTime)) + Math.Min(FlyTime, seconds % CycleTime);
                return (flySeconds * Speed);
            }
        }

        private long WinningDistance(string reindeerTxt, int seconds)
            => reindeerTxt.Lines().Select(Reindeer.Parse).Select(rd => rd.DistanceAfter(seconds)).Max();

        protected override long? Part1()
        {
            Assert(WinningDistance(Sample(), 10), 160);
            Assert(WinningDistance(Sample(), 1000), 1120);
            return WinningDistance(Input, 2503);
        }

        private long Race(string reindeerTxt, int rounds)
        {
            var reindeers = reindeerTxt.Lines().Select(Reindeer.Parse).ToList();

            for (int i = 1; i <= rounds; i++)
            {
                var winners = reindeers.GroupBy(r => r.DistanceAfter(i)).OrderByDescending(grp => grp.Key).First();
                foreach (var w in winners)
                    w.Score = w.Score + 1;
            }
            return reindeers.Select(r => r.Score).Max();
        }

        protected override long? Part2()
        {
            Assert(Race(Sample(), 1000), 689);
            return Race(Input, 2503);
        }
    }
}
