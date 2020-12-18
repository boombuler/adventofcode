using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2020_16
{
    class Program : ProgramBase
    {
        class Rule {
            long min1, min2, max1, max2;
            public string Name { get; set; }

            public bool Valid(long v) => (v >= min1 && v <= max1) || (v >= min2 && v <= max2);
        }

        static void Main(string[] args) => new Program().Run();

        private static Func<string, Rule> ParseRule =
            new Regex(@"(?<Name>[ \w]+): (?<min1>\d+)-(?<max1>\d+) or (?<min2>\d+)-(?<max2>\d+)").ToFactory<Rule>();

        private IEnumerable<Rule> GetRules(string inputFile)
        {
            foreach(var line in ReadLines(inputFile))
            {
                var m = ParseRule(line);
                if (m == null)
                    yield break;
                yield return m;
            }
        }

        private IEnumerable<long[]> OtherTickets(string inputFile)
        {
            foreach(var line in ReadLines(inputFile).SkipWhile(l => l != "nearby tickets:").Skip(1))
            {
                yield return line.Split(",").Select(long.Parse).ToArray();
            }
        }

        private long ErrorRate(IEnumerable<Rule> rules, long[] ticket)
        {
            var sum = 0L;
            foreach (var val in ticket)
            {
                if (!rules.Any(r => r.Valid(val)))
                    sum += val;
            }
            return sum;
        }

        private long GetErrorRate(string file)
        {
            var rules = GetRules(file).ToList();
            return OtherTickets(file).Sum(v => ErrorRate(rules, v));
        }

        protected override long? Part1()
        {
            Assert(GetErrorRate("Sample.txt"), 71);
            return GetErrorRate("Input.txt");
        }

        protected override long? Part2()
        {
            var file = "Input.txt";
            var rules = GetRules(file).ToList();
            var validTickets = OtherTickets(file).Where(t => ErrorRate(rules, t) == 0).ToList();

            var validColumns = rules.ToDictionary(
                r => r.Name, 
                r => new HashSet<int>(
                    Enumerable.Range(0, rules.Count)
                        .Where(col => !validTickets.Any(t => !r.Valid(t[col])))
                )
            );

            Dictionary<string, int> columns = new Dictionary<string, int>();

            foreach(var col in validColumns.OrderBy(kvp => kvp.Value.Count))
                columns[col.Key] = col.Value.Except(columns.Values).Single();

            var myTicket = ReadLines(file)
                .SkipWhile(l => l != "your ticket:").Skip(1)
                .First().Split(",").Select(long.Parse).ToArray();

            return columns
                .Where(kvp => kvp.Key.StartsWith("departure"))
                .Select(kvp => myTicket[kvp.Value])
                .Aggregate((a, b) => a * b);
        }
    }
}
