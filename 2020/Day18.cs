namespace AdventOfCode._2020;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day18 : Solution
{
    private static readonly Regex FindBraces = new(@"\([^\(\)]+\)", RegexOptions.Compiled);
    private long SolveWeirdMath(string term, bool withPrecedence)
    {
        string SolveSubTerm(string txt)
        {
            var tokens = txt.TrimStart('(').TrimEnd(')').Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            void ReduceAt(int idx)
            {
                var o1 = long.Parse(tokens[idx]);
                var op = tokens[idx + 1];
                var o2 = long.Parse(tokens[idx + 2]);
                switch (op)
                {
                    case "+": o1 += o2; break;
                    case "*": o1 *= o2; break;
                    default:
                        Error(string.Format("Unknown operator {0}", op));
                        break;
                }
                tokens[idx] = o1.ToString();
                tokens.RemoveRange(idx + 1, 2);
            }

            if (withPrecedence)
            {
                int idx;
                while ((idx = tokens.IndexOf("+")) >= 0)
                    ReduceAt(idx - 1);
            }

            while (tokens.Count > 1)
                ReduceAt(0);

            return tokens[0];
        }

        while (FindBraces.IsMatch(term))
            term = FindBraces.Replace(term, (m) => SolveSubTerm(m.Value));

        return long.Parse(SolveSubTerm(term));
    }

    protected override long? Part1()
    {
        Assert(SolveWeirdMath("1 + 2 * 3 + 4 * 5 + 6", false), 71);
        Assert(SolveWeirdMath("1 + (2 * 3) + (4 * (5 + 6))", false), 51);
        Assert(SolveWeirdMath("2 * 3 + (4 * 5)", false), 26);
        Assert(SolveWeirdMath("5 + (8 * 3 + 9 + 3 * 4 * 3)", false), 437);
        Assert(SolveWeirdMath("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", false), 12240);
        Assert(SolveWeirdMath("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", false), 13632);

        return Input.Lines().Select(n => SolveWeirdMath(n, false)).Sum();
    }

    protected override long? Part2()
    {
        Assert(SolveWeirdMath("1 + 2 * 3 + 4 * 5 + 6", true), 231);
        Assert(SolveWeirdMath("1 + (2 * 3) + (4 * (5 + 6))", true), 51);
        Assert(SolveWeirdMath("2 * 3 + (4 * 5)", true), 46);
        Assert(SolveWeirdMath("5 + (8 * 3 + 9 + 3 * 4 * 3)", true), 1445);
        Assert(SolveWeirdMath("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", true), 669060);
        Assert(SolveWeirdMath("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", true), 23340);

        return Input.Lines().Select(n => SolveWeirdMath(n, true)).Sum();
    }
}
