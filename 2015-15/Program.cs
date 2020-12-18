using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2015_15
{
    class Program : ProgramBase
    {
        struct Score
        {
            public int Capacity { get; }
            public int Durability { get; }
            public int Flavor { get; }
            public int Texture { get; }
            public int Calories { get; }

            public Score(int capacity, int durability, int flavor, int texture, int calories)
            {
                Capacity = capacity;
                Durability = durability;
                Flavor = flavor;
                Texture = texture;
                Calories = calories;
            }

            public Score Add(Score other)
                => new Score(Capacity + other.Capacity, Durability + other.Durability, Flavor + other.Flavor, Texture + other.Texture, Calories + other.Calories);
            public Score Mult(int factor)
                => new Score(Capacity * factor, Durability * factor, Flavor * factor, Texture * factor, Calories * factor);

            public int GetWeightedPoints() => Math.Max(Capacity, 0) * Math.Max(Durability, 0) * Math.Max(Flavor, 0) * Math.Max(Texture, 0);
        }

        static void Main(string[] args) => new Program().Run();


        private static readonly Regex Parse = new Regex(@"\w+: capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)");
        private long GetBestScore(string inputFile, Func<Score, bool> validate = null)
        {
            var allIngr = ReadLines(inputFile).Select(i =>
            {
                var m = Parse.Match(i);
                return new Score(
                    int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value),
                    int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value),
                    int.Parse(m.Groups[5].Value));
            }).ToList();

            if (validate == null)
                validate = s => true;

            return GetBestScore(new Score(), allIngr, 100, validate);
        }

        private long GetBestScore(Score baseScore, IEnumerable<Score> ingredients, int remainingSpoons, Func<Score, bool> validate)
        {
            if (!ingredients.Any())
                return 0;
            var score = ingredients.First();
            var other = ingredients.Skip(1);
            var otherCount = other.Count();
            if (otherCount == 0)
            {
                var total = score.Mult(remainingSpoons).Add(baseScore);
                if (validate(total))
                    return total.GetWeightedPoints();
                return 0;
            }

            long max = 0;
            for (int i = 0; i < remainingSpoons - otherCount; i++)
            {
                var remSpoons = remainingSpoons - i;

                var s = score.Mult(i).Add(baseScore);
                var best = GetBestScore(s, other, remSpoons, validate);
                if (best > max)
                    max = best;
            }
            return max;
        }

        protected override long? Part1()
        {
            Assert(GetBestScore("Sample.txt"), 62842880);
            return GetBestScore("Input.txt");
        }
        protected override long? Part2()
        {
            Func<Score, bool> validate = s => s.Calories == 500;
            Assert(GetBestScore("Sample.txt", validate), 57600000);
            return GetBestScore("Input.txt", validate);
        }
    }
}
