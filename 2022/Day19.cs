namespace AdventOfCode._2022;

using System.Collections;
using System.Diagnostics;

class Day19 : Solution
{
    record struct Amounts(int Ore, int Clay, int Obsidian, int Geodes) : IEnumerable<int>
    {
        public static Amounts operator +(Amounts m1, Amounts m2)
            => new Amounts(m1.Ore + m2.Ore, m1.Clay + m2.Clay, m1.Obsidian + m2.Obsidian, m1.Geodes + m2.Geodes);
        public static Amounts operator -(Amounts m1, Amounts m2)
            => new Amounts(m1.Ore - m2.Ore, m1.Clay - m2.Clay, m1.Obsidian - m2.Obsidian, m1.Geodes - m2.Geodes);
        public static Amounts operator *(Amounts m, int t)
            => new Amounts(m.Ore * t, m.Clay * t, m.Obsidian * t, m.Geodes * t);

        private IEnumerable<int> Enumerate()
        {
            yield return Ore;
            yield return Clay;
            yield return Obsidian;
            yield return Geodes;
        }

        public IEnumerator<int> GetEnumerator() => Enumerate().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();
    }

    record State(int TimeLeft, Amounts Resources, Amounts Robots);

    class Blueprint
    {
        public int ID { get; }

        private readonly List<(Amounts Costs, Amounts Robots)> fBuildOptions = new();
        private readonly int fMaxOreRate, fMaxClayRate, fMaxObsidianRate;

        public static readonly Func<string, Blueprint> Factory
            = new Regex(@"Blueprint (?<Id>\d+):\W+Each ore robot costs (?<OreRobotOre>\d+) ore.\W+Each clay robot costs (?<ClayRobotOre>\d+) ore.\W+Each obsidian robot costs (?<ObsidianRobotOre>\d+) ore and (?<ObsidianRobotClay>\d+) clay.\W+Each geode robot costs (?<GeodeRobotOre>\d+) ore and (?<GeodeRobotObsidian>\d+) obsidian\.", RegexOptions.Multiline).ToFactory<Blueprint>();

        public Blueprint(int Id, int OreRobotOre, int ClayRobotOre, int ObsidianRobotOre, int ObsidianRobotClay, int GeodeRobotOre, int GeodeRobotObsidian)
        {
            ID = Id;
            fBuildOptions.Add((new Amounts(OreRobotOre, 0, 0, 0), new Amounts(1, 0, 0, 0)));
            fBuildOptions.Add((new Amounts(ClayRobotOre, 0, 0, 0), new Amounts(0, 1, 0, 0)));
            fBuildOptions.Add((new Amounts(ObsidianRobotOre, ObsidianRobotClay, 0, 0), new Amounts(0, 0, 1, 0)));
            fBuildOptions.Add((new Amounts(GeodeRobotOre, 0, GeodeRobotObsidian, 0), new Amounts(0, 0, 0, 1)));            
            fMaxOreRate = new int[] { OreRobotOre, ClayRobotOre, ObsidianRobotOre, GeodeRobotOre }.Max();
            fMaxClayRate = ObsidianRobotClay;
            fMaxObsidianRate = GeodeRobotObsidian;
        }

        public long GetMaxGeodes(int Minutes)
        {
            var cache = new Dictionary<State, int>();
            int DFS(State state)
            {
                if (cache.TryGetValue(state, out var res))
                    return res;
                
                var maxGeodes = (state.Resources + (state.Robots * state.TimeLeft)).Geodes;

                foreach (var build in fBuildOptions)
                {
                    var newRobots = state.Robots + build.Robots;
                    if (newRobots.Ore > fMaxOreRate || newRobots.Obsidian > fMaxObsidianRate || newRobots.Clay > fMaxClayRate)
                        continue;

                    var missingResources = build.Costs - state.Resources;
                    int TimeRequired(int missing, int rate)
                        => (missing <= 0 ? 0 : rate <= 0 ? Minutes : (missing + rate - 1) / rate);
                    int requiredTime = missingResources.Zip(state.Robots, TimeRequired).Max() + 1;
                    int timeLeft = state.TimeLeft - requiredTime;

                    if (timeLeft <= 0)
                        continue;

                    var minedResource = state.Robots * requiredTime;
                    var totalResources = state.Resources - build.Costs + minedResource;

                    totalResources = totalResources with
                    {
                        Ore = Math.Min(totalResources.Ore, fMaxOreRate * timeLeft),
                        Clay = Math.Min(totalResources.Clay, fMaxClayRate * timeLeft),
                        Obsidian = Math.Min(totalResources.Obsidian, fMaxObsidianRate * timeLeft),
                    };

                    var geodes = DFS(new State(
                        timeLeft,
                        totalResources,
                        newRobots
                    ));
                    if (geodes > maxGeodes)
                        maxGeodes = geodes;
                }
                return cache[state] = maxGeodes;
            }

            return DFS(new State(Minutes, new Amounts(0, 0, 0, 0), new Amounts(1, 0, 0, 0)));
        }
    }

    protected override long? Part1()
    {
        static long SumQualityLevels(string input)
            => input.Lines().Select(Blueprint.Factory).Sum(bp => bp.ID * bp.GetMaxGeodes(24));

        Assert(SumQualityLevels(Sample()), 33);
        return SumQualityLevels(Input);
    }

    protected override long? Part2()
    {
        static long GetMaxGeodeCount(string input)
            => input.Lines().Select(Blueprint.Factory).Where(bp => bp.ID <= 3).Select(bp => bp.GetMaxGeodes(32)).Aggregate((a,b) => a*b);
        Assert(GetMaxGeodeCount(Sample()), 3472);
        return GetMaxGeodeCount(Input);
    }
}
