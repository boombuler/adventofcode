namespace AdventOfCode._2020;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day08 : Solution
{
    private static readonly Regex ParseOpCode = new(@"(?<op>\w{3}) (?<arg>[\+\-]\d+)", RegexOptions.Compiled);
    private static int RunCode(string codes)
    {
        var code = ParseCode(codes);

        return RunCode(code).ACC;
    }

    private static int FixCode(string codes)
    {
        var orgCode = ParseCode(codes);
        for (int i = 0; i < orgCode.Count; i++)
        {
            string patch;
            switch (orgCode[i].op)
            {
                case "nop": patch = "jmp"; break;
                case "jmp": patch = "nop"; break;
                default: continue;
            }

            var newCode = orgCode.ToList();
            newCode[i] = (patch, orgCode[i].arg);
            var (acc, finished) = RunCode(newCode);
            if (finished)
                return acc;
        }
        throw new InvalidOperationException("Code not fixable");
    }
    private static List<(string op, int arg)> ParseCode(string codes)
        => codes.Lines()
              .Select(line =>
              {
                  var oc = ParseOpCode.Match(line);
                  return (oc.Groups["op"].Value, int.Parse(oc.Groups["arg"].Value));
              })
              .ToList();

    private static (int ACC, bool Finished) RunCode(List<(string, int)> opCodes)
    {
        var executedLines = new HashSet<int>();
        int pc = 0;
        int acc = 0;
        while (!executedLines.Contains(pc))
        {
            executedLines.Add(pc);
            (var op, var arg) = opCodes[pc];
            switch (op)
            {
                case "jmp":
                    pc += arg;
                    break;
                case "acc":
                    acc += arg;
                    pc++;
                    break;
                default:
                    pc++;
                    break;
            }
            if (pc >= opCodes.Count)
            {
                return (acc, true);
            }
        }
        return (acc, false);
    }

    protected override long? Part1()
    {
        Assert(RunCode(Sample()), 5);
        return RunCode(Input);
    }

    protected override long? Part2()
    {
        Assert(FixCode(Sample()), 8);
        return FixCode(Input);
    }
}
