﻿using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2015
{
    class Day13 : Solution
    {
        private static readonly Regex ParseRule = new Regex(@"(?<a>\w+) would (?<gol>gain|lose) (?<amount>\d+) happiness units by sitting next to (?<b>\w+)\.");
        private Dictionary<(string, string), long> ParseRules(string rulesText)
        {
            var rules = new Dictionary<(string, string), long>();
            foreach (var line in rulesText.Lines())
            {
                var m = ParseRule.Match(line);
                if (!m.Success)
                    throw new Exception();

                long amount = long.Parse(m.Groups["amount"].Value);
                if (m.Groups["gol"].Value == "lose")
                    amount = -amount;

                rules[(m.Groups["a"].Value, m.Groups["b"].Value)] = amount;
            }
            return rules;
        }

        private long CheckHappiness(string[] seatOrder, Dictionary<(string, string), long> rules)
        {
            long result = 0;
            for (int i = 0; i < seatOrder.Length; i++)
            {
                var person = seatOrder[i];
                var left = i == 0 ? seatOrder.Last() : seatOrder[i - 1];
                var right = i == seatOrder.Length - 1 ? seatOrder.First() : seatOrder[i + 1];
                if (rules.TryGetValue((person, left), out long d))
                    result += d;
                if (rules.TryGetValue((person, right), out d))
                    result += d;
            }
            return result;
        }

        private long CheckBestSeatOrder(string inputFile, params string[] AdditionalPersons)
        {
            var rules = ParseRules(inputFile);
            var persons = AdditionalPersons.Union(rules.Keys.SelectMany(k => new string[] { k.Item1, k.Item2 })).Distinct();

            return persons.Permuatate().Select(so => CheckHappiness(so, rules)).Max();
        }

        protected override long? Part1()
        {
            Assert(CheckBestSeatOrder(Sample()), 330);
            return CheckBestSeatOrder(Input);
        }

        protected override long? Part2() => CheckBestSeatOrder(Input, "Me");
    }
}
