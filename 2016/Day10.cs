namespace AdventOfCode._2016;

class Day10 : Solution
{

    record Bot(string Name, string Low, string High) {
        public long? HoldValue { get; set; } 
    }

    private static readonly Func<string, Bot?> ParseBot =
        new Regex(@"(?<Name>bot \d+) gives low to (?<Low>(bot|output) \d+) and high to (?<High>(bot|output) \d+)", RegexOptions.Compiled).ToFactory<Bot>();

    class GiveChip { public required string Target; public long Value; }
    private static readonly Func<string, GiveChip?> ParseCommand =
        new Regex(@"value (?<Value>\d+) goes to (?<Target>(bot|output) \d+)", RegexOptions.Compiled).ToFactory<GiveChip>();

    private static (long min, long max) MinMax(long a, long b) => a > b ? (b, a) : (a, b);

    private static long FindBotComparing(string input, long checkLow, long checkHigh)
    {
        string comperator = string.Empty;
        RunBotnet(input, (bots, name, chip) =>
        {
            if (!bots.TryGetValue(name, out var bot) || !bot.HoldValue.HasValue)
                return true;
            (var lo, var hi) = MinMax(bot.HoldValue.Value, chip);
            if (lo == checkLow && hi == checkHigh)
            {
                comperator = bot.Name;
                return false;
            }
            return true;
        });
        return long.Parse(comperator.Split(' ')[1]);
    }

    private static long AggregateOutput(string input)
    {
        long result = 1;
        RunBotnet(input, (bots, name, chip) =>
        {
            if (name is "output 0" or "output 1" or "output 2")
                result *= chip;
            return true;
        });
        return result;
    }

    private static void RunBotnet(string input, Func<Dictionary<string, Bot>, string, long, bool> continueExection)
    {
        var bots = input.Lines().Select(ParseBot).NonNull().ToDictionary(b => b.Name);
        var commands = input.Lines().Select(ParseCommand).Where(c => c != null);
        var pending = new Queue<GiveChip>();

        foreach (var cmd in commands)
        {
            if (cmd == null)
                continue;
            pending.Enqueue(cmd);
            while (pending.TryDequeue(out var iv))
            {
                bots.TryGetValue(iv.Target, out var bot);
                if (!continueExection(bots, iv.Target, iv.Value))
                    return;

                if (iv.Target.StartsWith("output") || bot == null)
                    continue;

                if (!bot.HoldValue.HasValue)
                {
                    bot.HoldValue = iv.Value;
                    continue;
                }

                (var lo, var hi) = MinMax(bot.HoldValue.Value, iv.Value);

                bot.HoldValue = null;
                pending.Enqueue(new GiveChip() { Target = bot.Low, Value = lo });
                pending.Enqueue(new GiveChip() { Target = bot.High, Value = hi });
            }
        }
    }

    protected override long? Part1()
    {
        Assert(FindBotComparing(Sample(), 2, 5), 2);
        return FindBotComparing(Input, 17, 61);
    }

    protected override long? Part2()
    {
        Assert(AggregateOutput(Sample()), 30);
        return AggregateOutput(Input);
    }
}
