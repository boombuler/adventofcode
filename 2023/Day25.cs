namespace AdventOfCode._2023;

using static Parser;

class Day25 : Solution
{
    private static readonly Func<string, (string A, string B)[]> ParseInput = (s) =>
    (
        from fr in Word.Token() + ":"
        from toList in Word.Token().Many1()
        select toList.Select(to => (fr, to))
    ).List('\n').MustParse(s).SelectMany(res => res).ToArray();

    class Node(string name, IEnumerable<string> edges = null)
    {
        public List<string> Edges { get; } = (edges ?? Enumerable.Empty<string>()).ToList();

        public string Name { get; } = name;

        public int Value { get; private set; } = 1;

        public void MergeRandomEdge(Dictionary<string, Node> graph)
        {
            var other = graph[Edges.RandomElement()];
            Value += other.Value;
            Edges.AddRange(other.Edges);

            foreach (var n in other.Edges)
            {
                while (graph[n].Edges.Remove(other.Name))
                    graph[n].Edges.Add(Name);
            }
            Edges.RemoveAll(e => e == Name);

            graph.Remove(other.Name);
        }
    }

    private static long Solve(string input)
    {
        var edges = ParseInput(input).ToList();
        var graph = new Dictionary<string, Node>();
        foreach (var (a,b) in edges)
        {
            graph.GetOrAdd(a, () => new Node(a)).Edges.Add(b);
            graph.GetOrAdd(b, () => new Node(b)).Edges.Add(a);
        }

        while(true)
        {
            // Use Karger's Algorithm to randmoly contract the graph down to two nodes:
            var contractedGraph = graph.ToDictionary(
                 kvp => kvp.Key,
                 kvp => new Node(kvp.Value.Name, kvp.Value.Edges)
             );

            while (contractedGraph.Count > 2)
                contractedGraph.Values.RandomElement().MergeRandomEdge(contractedGraph);
            
            var (a, (b, _)) = contractedGraph.Values;
            if (a.Edges.Count == 3)
                return a.Value * b.Value;
        }
    }
    protected override long? Part1()
    {
        Assert(Solve(Sample()), 54);
        return Solve(Input);
    }
}
