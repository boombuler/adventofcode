namespace AdventOfCode._2017;

class Day24 : Solution
{
    record Connector(int A, int B)
    {
        public static readonly Func<string, Connector> Parse = new Regex(@"(?<A>\d+)/(?<B>\d+)").ToFactory<Connector>();
    }

    record Bridge
    {
        public int Tip { get; private init; }
        public ImmutableList<Connector> Connections { get; private init; } = ImmutableList<Connector>.Empty;

        public int Strength { get; private init; }
        public int Length => Connections.Count;

        public Bridge Push(Connector c)
            => new()
            {
                Tip = c.A == Tip ? c.B : c.A,
                Strength = Strength + c.A + c.B,
                Connections = Connections.Add(c)
            };
    }

    private static IEnumerable<Bridge> BuildBridges(string input)
    {
        var connections = input.Lines().Select(Connector.Parse)
            .SelectMany(c => new[] { (c.A, c), (c.B, c) })
            .ToLookup(x => x.Item1, x => x.c);

        var open = new Stack<Bridge>();
        open.Push(new Bridge());
        while (open.TryPop(out var bridge))
        {
            bool done = true;
            foreach (var option in connections[bridge.Tip].Except(bridge.Connections))
            {
                open.Push(bridge.Push(option));
                done = false;
            }
            if (done)
                yield return bridge;
        }
    }

    private static long GetStrongestBridge(string input)
        => BuildBridges(input).Max(b => b.Strength);

    protected override long? Part1()
    {
        Assert(GetStrongestBridge(Sample()), 31);
        return GetStrongestBridge(Input);
    }

    protected override long? Part2()
        => BuildBridges(Input).OrderByDescending(b => b.Length).ThenByDescending(b => b.Strength).First().Strength;
}
