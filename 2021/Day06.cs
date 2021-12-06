using System;
using System.Linq;

namespace AdventOfCode._2021
{
    class Day06 : Solution
    {
        const int Cooldown = 7;
        const int BreedCooldown = Cooldown + 2;

        private long Simulate(int Days)
        {
            var state     = new long[BreedCooldown];
            var nextState = new long[BreedCooldown];
            foreach (var grp in Input.Split(',').Select(int.Parse).GroupBy(n => n))
                state[grp.Key] = grp.Count();

            for (int day = 0; day < Days; day++)
            {
                Array.Copy(state, 1, nextState, 0, state.Length - 1);
                nextState[BreedCooldown - 1] = state[0];
                nextState[Cooldown - 1] += state[0];
                (state, nextState) = (nextState, state);
            }

            return state.Sum();
        }

        protected override long? Part1() => Simulate(80);

        protected override long? Part2() => Simulate(256);
    }
}
