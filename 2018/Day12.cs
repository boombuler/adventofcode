﻿namespace AdventOfCode._2018;

class Day12 : Solution
{
    const char EMPTY = '.';
    const char PLANT = '#';

    private static (string State, Dictionary<string, char> Rules) ReadRulesAndState(string data)
    {
        var (stateStr, ruleStr) = data.Lines();
        return (
            stateStr["initial state: ".Length..],
            ruleStr.Skip(1).Select(txt => txt.Split(" => ")).ToDictionary(s => s[0], s => s[1][0])
        );
    }

    private static (string State, long Offset) Step(string state, long offset, Dictionary<string, char> rules)
    {
        var res = new StringBuilder();
        const int PATTERN_WIDTH = 5;
        var margin = new string(EMPTY, PATTERN_WIDTH);
        state = string.Concat(margin, state, margin);
        for (int i = (PATTERN_WIDTH / 2); i < state.Length - (2 * (PATTERN_WIDTH / 2)); i++)
            res.Append(rules.TryGetValue(state.Substring(i - 2, PATTERN_WIDTH), out char c) ? c : EMPTY);

        state = res.ToString();
        int firstPlant = state.IndexOf(PLANT);

        return (state[firstPlant..].TrimEnd(EMPTY), offset + (PATTERN_WIDTH - (PATTERN_WIDTH / 2)) - firstPlant);
    }

    private static long SumPlants(string data, long gen)
    {
        var (state, rules) = ReadRulesAndState(data);
        long offset = 0;
        while (gen-- > 0)
        {
            var (next, nOffset) = Step(state, offset, rules);
            if (next == state)
            {
                offset = nOffset + (nOffset - offset) * gen;
                break;
            }
            (state, offset) = (next, nOffset);
        }

        return state.Select((v, i) => v == PLANT ? (i - offset) : 0).Sum();
    }

    protected override long? Part1()
    {
        Assert(SumPlants(Sample(), 20), 325);
        return SumPlants(Input, 20);
    }

    protected override long? Part2() => SumPlants(Input, 50_000_000_000);
}
