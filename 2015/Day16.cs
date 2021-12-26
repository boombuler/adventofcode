namespace AdventOfCode._2015;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day16 : Solution
{
    private static readonly Regex LinePattern = new(@"^Sue (?<n>\d+):\W(?<values>.*)$", RegexOptions.Compiled);
    private IEnumerable<int> CheckSues(Dictionary<string, Func<int, bool>> facts)
    {
        foreach (var line in Input.Lines())
        {
            var m = LinePattern.Match(line);
            int sue = int.Parse(m.Groups["n"].Value);
            string values = m.Groups["values"].Value;

            bool valid = true;
            foreach (var kvp in values.Split(','))
            {
                var itm = kvp.Split(":");
                var key = itm[0].Trim();
                var val = int.Parse(itm[1].Trim());

                if (!facts[key](val))
                {
                    valid = false;
                    break;
                }
            }
            if (valid)
                yield return sue;
        }
    }

    protected override long? Part1()
    {
        var facts = new Dictionary<string, Func<int, bool>>()
            {
                { "children", x => x == 3 },
                { "cats" , x => x ==7 },
                { "samoyeds" , x => x ==2 },
                { "pomeranians" , x => x ==3 },
                { "akitas" , x => x ==0 },
                { "vizslas" , x => x ==0 },
                { "goldfish" , x => x ==5 },
                { "trees" , x => x ==3 },
                { "cars" , x => x ==2 },
                { "perfumes" , x => x ==1 },
            };

        return CheckSues(facts).Single();
    }

    protected override long? Part2()
    {
        var facts = new Dictionary<string, Func<int, bool>>()
            {
                { "children", x => x == 3 },
                { "cats" , x => x > 7 },
                { "samoyeds" , x => x == 2 },
                { "pomeranians" , x => x < 3 },
                { "akitas" , x => x == 0 },
                { "vizslas" , x => x == 0 },
                { "goldfish" , x => x < 5 },
                { "trees" , x => x > 3 },
                { "cars" , x => x == 2 },
                { "perfumes" , x => x == 1 },
            };

        return CheckSues(facts).Single();
    }
}
