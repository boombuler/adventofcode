namespace AdventOfCode._2015;

class Day15 : Solution
{
    record struct Score(int Capacity, int Durability, int Flavor, int Texture, int Calories)
    {
        public readonly Score Add(Score other)
            => new(Capacity + other.Capacity, Durability + other.Durability, Flavor + other.Flavor, Texture + other.Texture, Calories + other.Calories);
        public readonly Score Mult(int factor)
            => new(Capacity * factor, Durability * factor, Flavor * factor, Texture * factor, Calories * factor);

        public readonly int GetWeightedPoints() => Math.Max(Capacity, 0) * Math.Max(Durability, 0) * Math.Max(Flavor, 0) * Math.Max(Texture, 0);
    }

    private static readonly Regex Parse = new (@"\w+: capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)");
    private long GetBestScore(string data, Func<Score, bool> validate = null)
    {
        var allIngr = data.Lines().Select(i =>
        {
            var m = Parse.Match(i);
            return new Score(
                int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value),
                int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value),
                int.Parse(m.Groups[5].Value));
        }).ToList();

        return GetBestScore(new Score(), allIngr, 100, validate ?? (s => true));
    }

    private static long GetBestScore(Score baseScore, IEnumerable<Score> ingredients, int remainingSpoons, Func<Score, bool> validate)
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
        Assert(GetBestScore(Sample()), 62842880);
        return GetBestScore(Input);
    }
    protected override long? Part2()
    {
        static bool validate(Score s) => s.Calories == 500;
        Assert(GetBestScore(Sample(), validate), 57600000);
        return GetBestScore(Input, validate);
    }
}
