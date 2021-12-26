namespace AdventOfCode._2021;

using System;
using System.Linq;

class Day06 : Solution
{
    const int COOLDOWN = 7;
    const int BREED_COOLDOWN = COOLDOWN + 2;

    private long Simulate(int Days)
    {
        var state = new long[BREED_COOLDOWN];
        var nextState = new long[BREED_COOLDOWN];
        foreach (var grp in Input.Split(',').Select(int.Parse).GroupBy(n => n))
            state[grp.Key] = grp.Count();

        for (int day = 0; day < Days; day++)
        {
            Array.Copy(state, 1, nextState, 0, state.Length - 1);
            nextState[BREED_COOLDOWN - 1] = state[0];
            nextState[COOLDOWN - 1] += state[0];
            (state, nextState) = (nextState, state);
        }

        return state.Sum();
    }

    protected override long? Part1() => Simulate(80);

    protected override long? Part2() => Simulate(256);
}
