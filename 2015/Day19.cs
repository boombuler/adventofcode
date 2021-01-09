using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode._2015
{
    class Day19 : Solution
    {
        private long GetUniqueReactions(IEnumerable<string> rules, string molecule)
        {
            var results = new HashSet<string>();
            var reactions = rules
                .Select(r => r.Split(" => "))
                .Select(p => new { From = p[0], To = p[1] });
            foreach (var reaction in reactions)
            {
                int i = 0;
                while ((i = molecule.IndexOf(reaction.From, i)) >= 0)
                {
                    var newMolecule = molecule.Substring(0, i) + reaction.To + molecule.Substring(i + reaction.From.Length);
                    results.Add(newMolecule);

                    i++;
                }
            }
            return results.Count;
        }

        private string InputMolecule => Input.Lines().Last();
        private IEnumerable<string> SampleRules => Sample().Lines();
        private IEnumerable<string> InputRules => Input.Lines().TakeWhile(l => !string.IsNullOrEmpty(l));

        protected override long? Part1()
        {
            Assert(GetUniqueReactions(SampleRules, "HOH"), 4);
            Assert(GetUniqueReactions(SampleRules, "HOHOHO"), 7);
            return GetUniqueReactions(InputRules, InputMolecule);
        }

        private IEnumerable<string> TokenizeMolecule(string molecule)
        {
            var cur = new StringBuilder();
            using(var mr = new StringReader(molecule))
            {
                while(mr.TryRead(out char c))
                {
                    if (char.IsUpper(c))
                    {
                        if (cur.Length > 0)
                            yield return cur.ToString();
                        cur.Clear();
                    }
                    cur.Append(c);
                }
            }
            if (cur.Length > 0)
                yield return cur.ToString();
        }

        protected override long? Part2() => TokenizeMolecule(InputMolecule).Aggregate(-1, (sum, m) => sum + m switch
        {
            "Rn" => 0,
            "Ar" => 0,
            "Y" => -1,
            _ => 1
        });
    }
}
