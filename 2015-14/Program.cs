using AdventHelper;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2015_14
{
    class Program : ProgramBase
    {
        class Reindeer
        {
            // Dancer can fly 16 km/s for 11 seconds, but then must rest for 162 seconds.
            private static Regex Description = new Regex(@"(?<name>\w+) can fly (?<speed>\d+) km/s for (?<flytime>\d+) seconds, but then must rest for (?<resttime>\d+) seconds\.");

            public string Name { get; }
            public int Speed { get; }
            public int FlyTime { get; }
            public int RestTime { get; }
            public int Score { get; set; }

            public int CycleTime => FlyTime + RestTime; 

            public Reindeer(string descpt)
            {
                var m = Description.Match(descpt);
                Name = m.Groups["name"].Value;
                Speed = int.Parse(m.Groups["speed"].Value);
                FlyTime = int.Parse(m.Groups["flytime"].Value);
                RestTime = int.Parse(m.Groups["resttime"].Value);
            }

            public long DistanceAfter(int seconds)
            {
                int flySeconds = (FlyTime * (seconds / CycleTime)) + Math.Min(FlyTime, seconds % CycleTime);
                return (flySeconds * Speed);
            }
        }

        private long WinningDistance(string inputFile, int seconds)
            => ReadLines(inputFile).Select(d => new Reindeer(d).DistanceAfter(seconds)).Max();

        static void Main(string[] args) => new Program().Run();

       

        protected override long? Part1()
        {
            Assert(WinningDistance("Sample.txt", 10), 160);
            Assert(WinningDistance("Sample.txt", 1000), 1120);
            return WinningDistance("Input.txt", 2503);
        }

        private long Race(string inputFile, int rounds)
        {
            var reindeers = ReadLines(inputFile).Select(d => new Reindeer(d)).ToList();

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
            Assert(Race("Sample.txt", 1000), 689);
            return Race("Input.txt", 2503);
        }
    }
}
