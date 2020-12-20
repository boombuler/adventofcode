using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2015_19
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private static readonly string INPUT_MOLECULE 
            = "ORnPBPMgArCaCaCaSiThCaCaSiThCaCaPBSiRnFArRnFArCaCaSiThCaCaSiThCaCaCaCaCaCaSiRnFYFArSiRnMgArCaSiRnPTiTiBFYPBFArSiRnCaSiRnTiRnFArSiAlArPTiBPTiRnCaSiAlArCaPTiTiBPMgYFArPTiRnFArSiRnCaCaFArRnCaFArCaSiRnSiRnMgArFYCaSiRnMgArCaCaSiThPRnFArPBCaSiRnMgArCaCaSiThCaSiRnTiMgArFArSiThSiThCaCaSiRnMgArCaCaSiRnFArTiBPTiRnCaSiAlArCaPTiRnFArPBPBCaCaSiThCaPBSiThPRnFArSiThCaSiThCaSiThCaPTiBSiRnFYFArCaCaPRnFArPBCaCaPBSiRnTiRnFArCaPRnFArSiRnCaCaCaSiThCaRnCaFArYCaSiRnFArBCaCaCaSiThFArPBFArCaSiRnFArRnCaCaCaFArSiRnFArTiRnPMgArF";

        private long GetUniqueReactions(string ruleFile, string molecule)
        {
            var results = new HashSet<string>();
            var reactions = ReadLines(ruleFile)
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

        protected override long? Part1()
        {
            Assert(GetUniqueReactions("Sample", "HOH"), 4);
            Assert(GetUniqueReactions("Sample", "HOHOHO"), 7);
            return GetUniqueReactions("Input", INPUT_MOLECULE);
        }

        protected override long? Part2()
        {
            Console.WriteLine("Solved by logic");
            return null;
        }
    }
}
