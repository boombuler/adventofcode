using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _2020_19
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private delegate IEnumerable<int> RuleTest(string input, IEnumerable<int> idx);

        private RuleTest TestChar(char c)
            => (str, idx) => idx.Where(i => str.Length >= i + 1 && str[i] == c).Select(i => i + 1);

        private RuleTest AND(RuleTest a, RuleTest b)
            => (s, i) => b(s, a(s, i));

        private RuleTest OR(RuleTest a, RuleTest b)
            => (s, i) =>
            {
                i = i.ToList();
                if (!i.Any())
                    return i;
                return a(s, i).Union(b(s, i)).Distinct();
            };

        private long CheckRule(string inputFile, string overrides = null)
        {
            var ruleTexts = ReadLines(inputFile).TakeWhile(l => !string.IsNullOrEmpty(l))
                .Select(l => l.Split(':', 2))
                .ToDictionary(s => int.Parse(s[0]), s => s[1].Trim());

            foreach (var ov in (overrides ?? string.Empty).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Split(':', 2)))
                ruleTexts[int.Parse(ov[0])] = ov[1].Trim();

            var ruleMatch = new Dictionary<int, RuleTest>();

            RuleTest BuildRule(int no)
            {
                if (ruleMatch.TryGetValue(no, out RuleTest res))
                    return res;

                var text = ruleTexts[no];
                if (text.StartsWith('"') && text.EndsWith('"') && text.Length == 3)
                    return ruleMatch[no] = TestChar(text[1]);

                RuleTest Lazy(int n) => (s, i) => BuildRule(n)(s, i);

                var rule = text.Split("|").Select(numtxt =>
                        numtxt.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse).Select(Lazy)
                        .Aggregate(AND)
                    ).Aggregate(OR);
                return ruleMatch[no] = rule;
            }

            var rule0 = BuildRule(0);
            Func<string, bool> isMatch = (line) => rule0(line, new int[] { 0 }).Any(l => l == line.Length);

            var startIdx = new int[] { 0 };

            return ReadLines(inputFile).SkipWhile(l => !string.IsNullOrEmpty(l))
                .Where(line => !string.IsNullOrEmpty(line)).Where(isMatch).Count();
        }


        protected override long? Part1()
        {
            Assert(CheckRule("Sample"), 2);
            return CheckRule("Input");
        }

        protected override long? Part2()
        {
            Assert(CheckRule("OwnSample"),2);

            var overrides = @"
8: 42 | 42 8
11: 42 31 | 42 11 31";
            return CheckRule("Input", overrides);
        }
    }
}
