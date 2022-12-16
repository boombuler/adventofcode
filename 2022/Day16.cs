namespace AdventOfCode._2022;

class Day16 : Solution
{
    record Valve(string Name, int FlowRate, string[] Tunnels)
    {
        public static readonly Func<string, Valve> Factory =
            new Regex(@"Valve (?<Name>\w+) has flow rate=(?<FlowRate>\d+); tunnels? leads? to valves? ((, )?(?<Tunnels>\w+))*").ToFactory<Valve>();
    }

    private int[,] Distances(List<Valve> valves)
    {
        int[,] distances = new int[valves.Count, valves.Count];
        for (int s = 0; s < valves.Count; s++)
        {
            var sVal = valves[s];
            for (int t = 0; t < valves.Count; t++)
            {
                if (s == t)
                    distances[s, t] = 0;
                else
                {
                    var tVal = valves[t];
                    if (sVal.Tunnels.Contains(tVal.Name))
                        distances[s, t] = 1;
                    else
                        distances[s, t] = int.MaxValue;
                }
            }
        }

        for (int k = 0; k < valves.Count; k++)
        {
            for (int i = 0; i < valves.Count; i++)
            {
                if (k == i || distances[i, k] == int.MaxValue)
                    continue;

                for (int j = 0; j < valves.Count; j++)
                {
                    if (k == j || distances[k, j] == int.MaxValue)
                        continue;

                    var d = distances[i, k] + distances[k, j];
                    if (distances[i, j] > d)
                        distances[i, j] = d;
                }
            }
        }
        return distances;
    }


    public long GetTotalFlowAmount(string input, int maxMinutes, bool withElephant)
    {
        var valves = input.Lines().Select(Valve.Factory).ToList();
        var targetValves = valves.Select((v, i) => (v.FlowRate, i)).Where(v => v.FlowRate > 0).Select(v => new { Index = v.i, Mask = 1L << v.i }).ToArray();
        var dists = Distances(valves);
        var startPt = valves.FindIndex(v => v.Name == "AA");        
        
        ((int, int) a, (int, int)? b) SortArgs((int Time, int Location) a, (int Time, int Location)? b)
        {
            if (b.HasValue && (b.Value.Time > a.Time || (b.Value.Time == a.Time && b.Value.Location > a.Location)))
                return (b.Value, a);
            return (a, b);
        }

        Func<(int, int), (int, int)?, long, long> getFlowAmount = null;
        long CalcFlowAmount((int Time, int Location) a, (int, int)? b, long openValves)
            => (
                from target in targetValves
                where (target.Mask & openValves) == 0
                let flowTime = a.Time - dists[a.Location, target.Index] - 1
                where flowTime > 0
                let args = SortArgs((flowTime, target.Index), b)
                select (flowTime * valves[target.Index].FlowRate) + getFlowAmount(args.a, args.b, openValves | target.Mask)
             ).DefaultIfEmpty(0).Max();

        getFlowAmount = new Func<(int, int), (int, int)?, long, long>(CalcFlowAmount).Memorize();

        return CalcFlowAmount((maxMinutes, startPt), withElephant ? (maxMinutes, startPt) : null, 0);
    }

    protected override long? Part1()
    {
        long Solve(string s) => GetTotalFlowAmount(s, 30, false);
        Assert(Solve(Sample()), 1651);
        return Solve(Input);
    }

  
    protected override long? Part2()
    {
        long Solve(string s) => GetTotalFlowAmount(s, 26, true);
        Assert(Solve(Sample()), 1707);
        return Solve(Input);
    }
}
