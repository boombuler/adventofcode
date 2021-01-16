using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2020
{
    class Day07 : Solution
    {
        class Stacking
        {
            public string Parent { get; }
            public string Content { get; }
            public int Count { get; }
            public Stacking(string parent, string content, int count)
                => (Parent, Content, Count) = (parent, content, count);
        }
        private const string SourceBag = "shiny gold";
        static readonly Regex GetParent = new Regex(@"(?<name>\w+ \w+) bags contain ");
        static readonly Regex GetValue = new Regex(@"(, )?(?<count>\d+) (?<name>\w+ \w+) bags?");

        private IEnumerable<Stacking> ReadRules(string ruleData)
            => ruleData.Lines()
                .Select(l => new { Line = l, Parent = GetParent.Match(l).Groups["name"].Value })
                .SelectMany(l =>
                    GetValue.Matches(l.Line)
                        .Select(m => new Stacking(l.Parent, m.Groups["name"].Value, int.Parse(m.Groups["count"].Value)))
                );

        private int CountParentBags(string ruleData)
        {
            var rules = ReadRules(ruleData).ToLookup(s => s.Content);

            HashSet<string> result = new HashSet<string>();
            Stack<string> toCheck = new Stack<string>();
            toCheck.Push(SourceBag);

            while(toCheck.TryPop(out var test))
            {
                foreach(var stacking in rules[test])
                {
                    if (!result.Contains(stacking.Parent))
                    {
                        toCheck.Push(stacking.Parent);
                        result.Add(stacking.Parent);
                    }
                }
            }
            return result.Count;
        }

        private int CountChildBags(string ruleData)
        {
            var rules = ReadRules(ruleData).ToLookup(s => s.Parent);

            int CountChildBags(string parent, int factor)
                => rules[parent].Select(s => s.Count + CountChildBags(s.Content, s.Count)).Sum() * factor;

            return CountChildBags(SourceBag, 1);
        }

        protected override long? Part1()
        {
            Assert(CountParentBags(Sample("1")), 4);
            return CountParentBags(Input);
        }
        protected override long? Part2()
        {
            Assert(CountChildBags(Sample("2")), 126);
            return CountChildBags(Input);
        }
    }
}
