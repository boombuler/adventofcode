namespace AdventOfCode._2022;

class Day11 : Solution
{
    class Monkey(int ID, long[] StartingItems, string Operant1, char Op, string Operant2, long Test, int TrueTarget, int FalseTarget)
    {
        private readonly long? fOperant1 = Operant1 is "old" ? null : long.Parse(Operant1); 
        private readonly long? fOperant2 = Operant2 is "old" ? null : long.Parse(Operant2);

        public int ID { get; } = ID;
        public long[] StartingItems { get; } = StartingItems;
        public long Test { get; } = Test;

        public static readonly Func<string, Monkey> Factory =
            new Regex(@"Monkey (?<ID>\d+):\W*Starting items:(\W*(?<StartingItems>\d+),?)*\W*Operation: new = (?<Operant1>old|\d+) (?<Op>[\+\*]) (?<Operant2>old|\d+)\W*Test: divisible by (?<Test>\d+)\W*If true: throw to monkey (?<TrueTarget>\d+)\W*If false: throw to monkey (?<FalseTarget>\d+)", RegexOptions.Compiled|RegexOptions.Multiline).ToFactory<Monkey>();

        public (int TargetId, long NewLevel) Throw(long level, Func<long, long> relief)
        {
            var newLvl = Op switch
            {
                '+' => (fOperant1 ?? level) + (fOperant2 ?? level),
                '*' => (fOperant1 ?? level) * (fOperant2 ?? level),
                _ => throw new NotImplementedException()
            };
            newLvl = relief(newLvl);
            return ((newLvl % Test == 0) ? TrueTarget : FalseTarget, newLvl);
        }
    }

    private static long ObserveMonkeys(string input, int rounds, Func<IEnumerable<Monkey>, Func<long, long>> getRelief)
    {
        var monkeys = input.Split("\n\n").Select(Monkey.Factory).ToArray();
        var items = monkeys.Select(m => new Queue<long>(m.StartingItems)).ToArray();
        long[] activity = new long[monkeys.Length];
        var relief = getRelief(monkeys);

        for (int round = 0; round < rounds; round++)
        {
            for(int m = 0; m < monkeys.Length; m++)
            {
                while (items[m].TryDequeue(out long itm))
                {
                    (int target, itm) = monkeys[m].Throw(itm, relief);
                    items[target].Enqueue(itm);
                    activity[m]++;
                }
            }
        }
        return activity.OrderDescending().Take(2).Aggregate((a,b) => a*b);
    }

    protected override long? Part1() 
    {
        static long observe(string input)
            => ObserveMonkeys(input, 20, _ => x => x/3);
        Assert(observe(Sample()), 10605);
        return observe(Input);
    }

    protected override long? Part2()
    {
        static long observe(string input) 
            => ObserveMonkeys(input, 10_000, monkeys =>
            {
                var lcm = monkeys.Select(m => m.Test).Aggregate(MathExt.LCM);
                return v => v % lcm;
            });
        Assert(observe(Sample()), 2713310158);
        return observe(Input);
    }
}
