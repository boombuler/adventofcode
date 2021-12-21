using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2016
{
    class Day10 : Solution
    {
#pragma warning disable 0649
        class Bot { public string Name; public string Low; public string High; public long? HoldValue; }
#pragma warning restore 0649
        private static readonly Func<string, Bot> ParseBot = 
            new Regex(@"(?<Name>bot \d+) gives low to (?<Low>(bot|output) \d+) and high to (?<High>(bot|output) \d+)", RegexOptions.Compiled).ToFactory<Bot>();

        class GiveChip { public string Target; public long Value; }
        private static readonly Func<string, GiveChip> ParseCommand = 
            new Regex(@"value (?<Value>\d+) goes to (?<Target>(bot|output) \d+)", RegexOptions.Compiled).ToFactory<GiveChip>();

        private (long min, long max) MinMax(long a, long b) => a > b ? (b, a) : (a, b);

        private long FindBotComparing(string input, long checkLow, long checkHigh)
        {
            string comperator = null;
            RunBotnet(input, (bots, name, chip) =>
            {
                if (!bots.TryGetValue(name, out Bot bot) || !bot.HoldValue.HasValue)
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

        private long AggregateOutput(string input)
        {
            long result = 1;
            RunBotnet(input, (bots, name, chip) =>
            {
                if (name == "output 0" || name == "output 1" || name == "output 2")
                    result *= chip;
                return true;
            });
            return result;
        }

        private void RunBotnet(string input,  Func<Dictionary<string, Bot>, string, long, bool> continueExection)
        {
            var bots = input.Lines().Select(ParseBot).Where(b => b != null).ToDictionary(b => b.Name);
            var commands = input.Lines().Select(ParseCommand).Where(c => c != null);
            var pending = new Queue<GiveChip>();

            foreach (var cmd in commands)
            {
                pending.Enqueue(cmd);
                while (pending.TryDequeue(out GiveChip iv))
                {
                    bots.TryGetValue(iv.Target, out Bot bot);
                    if (!continueExection(bots, iv.Target, iv.Value))
                        return;

                    if (iv.Target.StartsWith("output"))
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
}
