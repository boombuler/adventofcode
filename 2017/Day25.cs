namespace AdventOfCode._2017;

class Day25 : Solution
{
    enum Direction { left, right };

    record StateMachine(string InitialState, int Rounds, State[] States);
    record State(string Name, Operation[] Operations);
    record Operation(int Condition, int Value, Direction Direction, string NextState);
    private static readonly Func<string, Operation> ParseOperation = new Regex(@"If the current value is (?<Condition>\d+):\n\s*- Write the value (?<Value>\d+)\.\n\s*- Move one slot to the (?<Direction>left|right)\.\n\s*- Continue with state (?<NextState>\w+)\.", RegexOptions.Compiled).ToFactory<Operation>();
    private static readonly Regex ParseInitialState = new(@"Begin in state (?<InitialState>\w+)\.", RegexOptions.Compiled);
    private static readonly Regex ParseRounds = new(@"Perform a diagnostic checksum after (?<Rounds>\d+) steps\.", RegexOptions.Compiled);
    private static readonly Regex ParseStateName = new(@"In state (?<Name>\w+):", RegexOptions.Compiled);
    private static StateMachine LoadStateMachine(string input)
    {
        var parts = input.Split("\n\n");
        var initialState = ParseInitialState.Match(parts[0]).Groups["InitialState"].Value;
        var rounds = int.Parse(ParseRounds.Match(parts[0]).Groups["Rounds"].Value);
        var states = new List<State>();
        foreach (var stateDescrpt in parts.Skip(1))
        {
            var name = ParseStateName.Match(stateDescrpt.Lines().First()).Groups["Name"].Value;
            var ops = stateDescrpt.Lines().Skip(1).Chunk(4).Select(grp => string.Join("\n", grp)).Select(ParseOperation);
            states.Add(new State(name, ops.ToArray()));
        }
        return new StateMachine(initialState, rounds, states.ToArray());
    }

    private static long RunTuring(string machineDescript)
    {
        var sm = LoadStateMachine(machineDescript);
        var band = new LinkedList<int>();
        var current = band.AddLast(0);

        var state = sm.InitialState;
        for (int round = 0; round < sm.Rounds; round++)
        {
            var s = sm.States.First(s => s.Name == state);
            var op = s.Operations.First(o => o.Condition == current.Value);
            state = op.NextState;
            current.Value = op.Value;

            if (op.Direction == Direction.right)
                current = current.Next ?? band.AddLast(0);
            else
                current = current.Previous ?? band.AddFirst(0);
        }
        return band.Count(v => v == 1);
    }

    protected override long? Part1()
    {
        Assert(RunTuring(Sample()), 3);
        return RunTuring(Input);
    }
}
