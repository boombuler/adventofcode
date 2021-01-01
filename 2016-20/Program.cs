using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2016_20
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        public IEnumerable<(long lower, long upper)> ParseRules(string ruleFile)
        {
            var rules = ReadLines(ruleFile)
                .Select(l => l.Split('-').Select(long.Parse).ToArray())
                .Select(r => (lower: r[0], upper: r[1]+1))
                .OrderBy(r => r.lower);

            var curRange = rules.First();
            foreach(var rule in rules.Skip(1))
            {
                if (rule.lower > curRange.upper)
                {
                    yield return curRange;
                    curRange = rule;
                }
                else
                    curRange.upper = Math.Max(curRange.upper, rule.upper);
            }
            yield return curRange;
        }

        public long LowestPossibleIP(string ruleFile)
        {
            var firstRule = ParseRules(ruleFile).First();
            if (firstRule.lower == 0)
                return firstRule.upper;
            return 0;
        }

        public long CountValidIPs(string ruleFile, long maxIP = uint.MaxValue)
        {
            var result = ParseRules(ruleFile).Aggregate((ip: 0L, count: 0L),
                (res, rule) => (rule.upper, res.count + (rule.lower - res.ip))
            );
            return result.count + (Math.Max(0, maxIP - result.ip));
        }

        protected override long? Part1()
        {
            Assert(LowestPossibleIP("Sample"), 3u);
            return LowestPossibleIP("Input");
        }

        protected override long? Part2() => CountValidIPs("Input");
    }
}
