using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2020_08
{
    class Program : ProgramBase
    {
        private static readonly Regex ParseOpCode = new Regex(@"(?<op>\w{3}) (?<arg>[\+\-]\d+)", RegexOptions.Compiled);
        private int RunCode(string codeFile)
        {
            var code = ParseCode(codeFile);

            return RunCode(code).ACC;
        }

        private int FixCode(string codeFile)
        {
            var orgCode = ParseCode(codeFile);
            for (int i = 0; i < orgCode.Count; i++)
            {
                string patch = null;
                switch(orgCode[i].op)
                {
                    case "nop": patch = "jmp"; break;
                    case "jmp": patch = "nop"; break;
                    default: continue;
                }

                var newCode = orgCode.ToList();
                newCode[i] = (patch, orgCode[i].arg);
                var res = RunCode(newCode);
                if (res.Finished)
                    return res.ACC;
            }
            throw new InvalidOperationException("Code not fixable");
        }
        private List<(string op, int arg)> ParseCode(string codeFile)
        {
            return ReadLines(codeFile)
                            .Select(line =>
                            {
                                var oc = ParseOpCode.Match(line);
                                return (oc.Groups["op"].Value, int.Parse(oc.Groups["arg"].Value));
                            })
                            .ToList();
        }

        private (int ACC, bool Finished) RunCode(List<(string, int)> opCodes)
        {
            var executedLines = new HashSet<int>();
            int PC = 0;
            int ACC = 0;
            while (!executedLines.Contains(PC))
            {
                executedLines.Add(PC);
                (var op, var arg) = opCodes[PC];
                switch (op)
                {
                    case "jmp":
                        PC += arg;
                        break;
                    case "acc":
                        ACC += arg;
                        PC++;
                        break;
                    default:
                        PC++;
                        break;
                }
                if (PC >= opCodes.Count)
                {
                    return (ACC, true);
                }
            }
            return (ACC, false);
        }
        
        static void Main(string[] args) => new Program().Run();

        protected override long? Part1()
        {
            Assert(RunCode("SampleInput.txt"), 5);
            return RunCode("Input.txt");
        }

        protected override long? Part2()
        {
            Assert(FixCode("SampleInput.txt"), 8);
            return FixCode("Input.txt");
        }
    }
}
