namespace AdventOfCode._2022;

using System.Collections;

class Day19 : Solution
{
    record struct Amounts(int Ore, int Clay, int Obsidian) : IEnumerable<int>
    {
        public static Amounts operator +(Amounts m1, Amounts m2)
            => new (m1.Ore + m2.Ore, m1.Clay + m2.Clay, m1.Obsidian + m2.Obsidian);
        public static Amounts operator -(Amounts m1, Amounts m2)
            => new (m1.Ore - m2.Ore, m1.Clay - m2.Clay, m1.Obsidian - m2.Obsidian);
        public static Amounts operator *(Amounts m, int t)
            => new (m.Ore * t, m.Clay * t, m.Obsidian * t);

        private readonly IEnumerable<int> Enumerate()
        {
            yield return Ore;
            yield return Clay;
            yield return Obsidian;
        }

        public readonly IEnumerator<int> GetEnumerator() => Enumerate().GetEnumerator();
        readonly IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();
    }

    record State(int TimeLeft, Amounts Resources, Amounts Robots);

    class Blueprint
    {
        public int ID { get; }

        private readonly List<(Amounts Costs, Amounts Robots)> fBuildOptions = [];
        private readonly Amounts fGeodeBotCost;
        private readonly Amounts fMaxRates;

        public static readonly Func<string, Blueprint> Factory
            = new Regex(@"Blueprint (?<Id>\d+):\W+Each ore robot costs (?<OreRobotOre>\d+) ore.\W+Each clay robot costs (?<ClayRobotOre>\d+) ore.\W+Each obsidian robot costs (?<ObsidianRobotOre>\d+) ore and (?<ObsidianRobotClay>\d+) clay.\W+Each geode robot costs (?<GeodeRobotOre>\d+) ore and (?<GeodeRobotObsidian>\d+) obsidian\.", RegexOptions.Multiline).ToFactory<Blueprint>();

        public Blueprint(int Id, int OreRobotOre, int ClayRobotOre, int ObsidianRobotOre, int ObsidianRobotClay, int GeodeRobotOre, int GeodeRobotObsidian)
        {
            ID = Id;
            fBuildOptions.Add((new Amounts(OreRobotOre, 0, 0), new Amounts(1, 0, 0)));
            fBuildOptions.Add((new Amounts(ClayRobotOre, 0, 0), new Amounts(0, 1, 0)));
            fBuildOptions.Add((new Amounts(ObsidianRobotOre, ObsidianRobotClay, 0), new Amounts(0, 0, 1)));
            fGeodeBotCost = new Amounts(GeodeRobotOre, 0, GeodeRobotObsidian);
            fMaxRates = new Amounts(
                new int[] { OreRobotOre, ClayRobotOre, ObsidianRobotOre, GeodeRobotOre }.Max(),
                ObsidianRobotClay,
                GeodeRobotObsidian
            );
        }

        public long GetMaxGeodes(int Minutes)
        {
            var cache = new Dictionary<State, int>();
            int DFS(State state)
            {
                bool TryBuild(Amounts reciep, out int timeLeft, out Amounts newResources)
                {
                    var missingResources = reciep - state.Resources;
                    int TimeRequired(int missing, int rate)
                        => (missing <= 0 ? 0 : rate <= 0 ? Minutes : (missing + rate - 1) / rate);
                    var requiredTime = missingResources.Zip(state.Robots, TimeRequired).Max() + 1;
                    timeLeft = state.TimeLeft - requiredTime;
                    
                    if (timeLeft <= 0)
                    {
                        newResources = new Amounts();
                        return false;
                    }

                    var minedResource = state.Robots * requiredTime;
                    newResources = state.Resources - reciep + minedResource;

                    newResources = newResources with
                    {
                        Ore = Math.Min(newResources.Ore, fMaxRates.Ore * timeLeft),
                        Clay = Math.Min(newResources.Clay, fMaxRates.Clay * timeLeft),
                        Obsidian = Math.Min(newResources.Obsidian, fMaxRates.Obsidian * timeLeft),
                    };
                    return true;
                }

                if (state.TimeLeft <= 1)
                    return 0;

                if (cache.TryGetValue(state, out var res))
                    return res;

                var maxGeodes = 0;

                // Try to build Geode Bot
                if (TryBuild(fGeodeBotCost, out var timeLeft, out var newResources))
                {
                    maxGeodes = timeLeft
                        + DFS(state with
                        {
                            TimeLeft = timeLeft,
                            Resources = newResources
                        });
                }

                foreach (var (costs, robots) in fBuildOptions)
                {
                    var newRobots = state.Robots + robots;
                    if (newRobots.Zip(fMaxRates).Any(t => t.First > t.Second))
                        continue;

                    if (TryBuild(costs, out timeLeft, out newResources))
                    {
                        var geodes = DFS(new State(
                            timeLeft,
                            newResources,
                            newRobots
                        ));
                        if (geodes > maxGeodes)
                            maxGeodes = geodes;
                    }
                }
                return cache[state] = maxGeodes;
            }

            return DFS(new State(Minutes, new Amounts(0, 0, 0), new Amounts(1, 0, 0)));
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
