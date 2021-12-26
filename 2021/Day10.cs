﻿namespace AdventOfCode._2021;

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day10 : Solution
{
    private readonly char[] Open = new char[] { '(', '[', '{', '<' };
    private readonly char[] Close = new char[] { ')', ']', '}', '>' };
    private readonly int[] Score = new int[] { 3, 57, 1197, 25137 };

    (long? Corruption, long? AutoComplete) CheckLine(string line)
    {
        var state = new Stack<int>();
        foreach (var c in line)
        {
            var idx = Array.IndexOf(Open, c);
            if (idx >= 0)
                state.Push(idx);
            else
            {
                idx = Array.IndexOf(Close, c);
                if (state.Count == 0 || state.Pop() != idx)
                    return (Score[idx], null);
            }
        }
        return (null, state.Aggregate(0L, (sum, idx) => sum * 5 + (idx + 1)));
    }

    long CorruptionScore(string input) => input.Lines().Select(CheckLine).Where(r => r.Corruption.HasValue).Sum(r => r.Corruption.Value);

    long CompletionScore(string input)
    {
        var completionLines = input.Lines().Select(CheckLine)
            .Where(r => r.AutoComplete.HasValue)
            .Select(r => r.AutoComplete.Value)
            .OrderBy(v => v)
            .ToList();
        return completionLines[completionLines.Count / 2];
    }

    protected override long? Part1()
    {
        Assert(CorruptionScore(Sample()), 26397);
        return CorruptionScore(Input);
    }

    protected override long? Part2()
    {
        Assert(CompletionScore(Sample()), 288957);
        return CompletionScore(Input);
    }
}
