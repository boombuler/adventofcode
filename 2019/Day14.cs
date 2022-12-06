namespace AdventOfCode._2019;

class Day14 : Solution
{
    record MatAmount(long Amount, string Material)
    {
        public MatAmount(string txt)
            : this(int.Parse(txt.Split(' ')[0]), txt.Split(' ')[1])
        {
        }

        public static MatAmount operator *(MatAmount a, long c) => new(a.Amount * c, a.Material);
    }

    private static long GetRequiredOre(string reactionText, long fuelAmount = 1)
    {
        var reactions = (
            from reaction in reactionText.Lines()
            let parts = reaction.Split(" => ")
            let input = parts[0].Split(", ").Select(r => new MatAmount(r))
            select new { Input = input.ToArray(), Output = new MatAmount(parts[1]) }
        ).ToDictionary(r => r.Output.Material);

        long oreAmount = 0;
        var remainders = reactions.Keys.ToDictionary(k => k, k => 0L);

        var missing = new Queue<MatAmount>();
        missing.Enqueue(new MatAmount(fuelAmount, "FUEL"));

        while (missing.Any())
        {
            var (requiredAmount, mat) = missing.Dequeue();
            if (mat == "ORE")
            {
                oreAmount += requiredAmount;
                continue;
            }

            var take = Math.Min(remainders[mat], requiredAmount);
            remainders[mat] -= take;
            requiredAmount -= take;

            if (requiredAmount == 0)
                continue;

            var reciep = reactions[mat];
            var times = Math.DivRem(requiredAmount, reciep.Output.Amount, out var partial);
            if (partial > 0)
                times++;

            remainders[mat] += (times * reciep.Output.Amount) - requiredAmount;

            foreach (var i in reciep.Input)
                missing.Enqueue(i * times);
        }

        return oreAmount;
    }

    private static long MaxFuel(string reactionText, long TotalOre = 1_000_000_000_000)
    {
        long a = 1;
        long b = TotalOre;

        while (true)
        {
            var diff = (b - a) / 2;
            if (diff == 0)
                return a;
            long test = a + diff;
            if (GetRequiredOre(reactionText, test) > TotalOre)
                b = test;
            else
                a = test;
        }
    }

    protected override long? Part1()
    {
        Assert(GetRequiredOre(Sample("0")), 31);
        Assert(GetRequiredOre(Sample("1")), 165);
        Assert(GetRequiredOre(Sample("2")), 13312);
        Assert(GetRequiredOre(Sample("3")), 180697);
        Assert(GetRequiredOre(Sample("4")), 2210736);
        return GetRequiredOre(Input);
    }

    protected override long? Part2()
    {
        Assert(MaxFuel(Sample("2")), 82892753);
        Assert(MaxFuel(Sample("3")), 5586022);
        Assert(MaxFuel(Sample("4")), 460664);
        return MaxFuel(Input);
    }
}
