namespace AdventOfCode._2024;

using static Parser;

class Day24 : Solution<long?, string>
{
    record Gate(string A, string B, GateType Type);
    enum GateType { AND, OR, XOR };
    private static Parser<string> GateName => Expect(char.IsLetterOrDigit).Many1().Text();

    private static Parser<KeyValuePair<string, bool>> StateParser =>
        from name in GateName + ": "
        from state in Char('1', true).Or(Char('0', false))
        select new KeyValuePair<string,bool>(name, state);

    private static Parser<KeyValuePair<string, Gate>> GateParser =>
        from a in GateName.Token()
        from type in Enum<GateType>()
        from b in GateName.Token() + "->"
        from o in GateName.Token()
        select new KeyValuePair<string, Gate>(o, new Gate(a, b, type));

    private static Parser<(Dictionary<string, bool> State, ImmutableDictionary<string, Gate> Gates)> InputParser =>
        from states in StateParser.List('\n') + "\n\n"
        from gates in GateParser.List('\n')
        select (states.ToDictionary(), gates.ToImmutableDictionary());

    private static long Evaluate(Dictionary<string, bool> state, ImmutableDictionary<string, Gate> gates)
    {
        var open = gates.ToDictionary();
        while (open.Count > 0)
        {
            var calculatable = open.Where(g => !open.ContainsKey(g.Value.A) && !open.ContainsKey(g.Value.B));

            bool hasCalculated = false;
            foreach (var (c, gate) in calculatable)
            {
                hasCalculated = true;
                state[c] = gate.Type switch
                {
                    GateType.AND => state[gate.A] && state[gate.B],
                    GateType.OR => state[gate.A] || state[gate.B],
                    GateType.XOR => state[gate.A] ^ state[gate.B],
                    _ => throw new InvalidOperationException(),
                };
                open.Remove(c);
            }
            if (!hasCalculated)
                return -1;
        }

        return state.Where(s => s.Key[0] == 'z' && s.Value).Select(s => int.Parse(s.Key[1..]))
            .Aggregate(0L, (acc, i) => acc | (1L << i));
    }

    protected override long? Part1()
    {
        static long Solve(string input)
        {
            var (state, gates) = InputParser.MustParse(input);
            return Evaluate(state, gates);
        }

        Assert(Solve(Sample()), 2024);
        return Solve(Input);
    }

    protected override string Part2() 
    {
        IEnumerable<KeyValuePair<string, bool>> ToState(long num, char varName)
            => Enumerable.Range(0, 64).Select(bit => new KeyValuePair<string, bool>(varName + bit.ToString("d2"), ((num >> bit) & 1) == 1));

        long RunSim(long x, long y, ImmutableDictionary<string, Gate> gates)
            => Evaluate(ToState(x, 'x').Concat(ToState(y, 'y')).ToDictionary(), gates);

        bool IsBitAdderFunctional(int bit, ImmutableDictionary<string, Gate> gates)
        {
            var m = 1L << bit;
            var halfAdderOk = Enumerable.Zip([0, 0, 1, 1], [0, 1, 0, 1])
                .All(x => ((RunSim(x.First * m, x.Second * m, gates) & m) == m) == ((x.First ^ x.Second) == 1));
            if (!halfAdderOk || bit == 0)
                return halfAdderOk;

            var c = 1L << (bit - 1);
            return Enumerable.Zip([0, 0, 1, 1], [0, 1, 0, 1])
                .All(x => ((RunSim((x.First * m)|c, (x.Second * m)|c, gates) & m) == m) == ((x.First ^ x.Second ^ 1) == 1));
        }

        IEnumerable<string> GetRequiredGates(string destination, ImmutableDictionary<string, Gate> gates)
        {
            var open = new Queue<string>([destination]);
            var seen = new HashSet<string>();
            while (open.TryDequeue(out var cur))
            {
                if (!gates.TryGetValue(cur, out var g))
                    continue;

                yield return cur;
                open.Enqueue(g.A);
                open.Enqueue(g.B);
            }
        }

        var (inputVars, gates) = InputParser.MustParse(Input);
        int bitCount = inputVars.Max(v => int.Parse(v.Key[1..]));

        IEnumerable<ImmutableList<string>> RepairWires(int testBit, ImmutableDictionary<string, Gate> gates, ImmutableHashSet<string> testedWires)
        {
            if (testBit >= bitCount) 
                yield return ImmutableList<string>.Empty;

            var usedGates = GetRequiredGates($"z{testBit:d2}", gates);
            var usedAndTested = testedWires.Union(usedGates);

            if (IsBitAdderFunctional(testBit, gates))
            {
                foreach(var r in RepairWires(testBit + 1, gates, usedAndTested))
                    yield return r;
            }

            foreach (var g1 in usedGates.Where(n => !testedWires.Contains(n)))
            {
                foreach (var g2 in gates.Keys.Where(n => !testedWires.Contains(n)))
                {
                    var newNet = gates.SetItems([
                        new KeyValuePair<string, Gate>(g1, gates[g2]),
                        new KeyValuePair<string, Gate>(g2, gates[g1])
                    ]);
                    if (IsBitAdderFunctional(testBit, newNet))
                    {
                        foreach(var r in RepairWires(testBit + 1, newNet, usedAndTested.Union([g1, g2])))
                            yield return r.AddRange([g1, g2]);
                    }
                }
            }
        }

        return string.Join(",", RepairWires(0, gates, []).First(n => n.Count <= 8).Order());
    }
}
