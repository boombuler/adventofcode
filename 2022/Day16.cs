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
        foreach (var p in Point2D.Range((0, 0), (valves.Count - 1, valves.Count - 1)))
            distances[p.X, p.Y] = (p.X == p.Y) ? 0 :
                (valves[(int)p.X].Tunnels.Contains(valves[(int)p.Y].Name) ? 1 : int.MaxValue);

        foreach(var (k, i, j) in Point3D.Range((0, 0, 0), (valves.Count - 1, valves.Count - 1, valves.Count - 1)))
        {
            if (k == i || k == j || distances[k, j] == int.MaxValue || distances[i, k] == int.MaxValue)
                continue;
            var d = distances[i, k] + distances[k, j];
            if (distances[i, j] > d)
                distances[i, j] = d;
        }
        return distances;
    }

    record struct State(int Time, int Location);

    public long GetTotalFlowAmount(string input, int maxMinutes, bool withElephant)
    {
        var valves = input.Lines().Select(Valve.Factory).ToList();
        var targetValves = valves.Select((v, i) => (v.FlowRate, i)).Where(v => v.FlowRate > 0).Select(v => new { Index = v.i, Mask = 1L << v.i }).ToArray();
        var dists = Distances(valves);
        var startPt = valves.FindIndex(v => v.Name == "AA");        
        
        (State a, State b) SortArgs(State a, State b) => a.Time > b.Time || (a.Time == b.Time && a.Location >= b.Location ) ? (a, b) : (b, a);

        Func<State, State, long, long> getFlowAmount = null;
        long CalcFlowAmount(State a, State b, long openValves)
            => (
                from target in targetValves
                where (target.Mask & openValves) == 0
                let flowTime = a.Time - dists[a.Location, target.Index] - 1
                where flowTime > 0
                let args = SortArgs(new State(flowTime, target.Index), b)
                select (flowTime * valves[target.Index].FlowRate) + getFlowAmount(args.a, args.b, openValves | target.Mask)
             ).DefaultIfEmpty(0).Max();

        getFlowAmount = new Func<State, State, long, long>(CalcFlowAmount).Memorize();

        return CalcFlowAmount(new State(maxMinutes, startPt), new State(withElephant ? maxMinutes : -1, startPt) , 0);
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
