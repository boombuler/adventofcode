namespace AdventOfCode._2017;

class Day07 : Solution<string, int>
{
    class Node
    {
        public static readonly Func<string, Node?> Parse = new Regex(@"(?<Name>\w+) \((?<Weight>\d+)\)(\s*->\s*((?<ChildNames>\w+)(,?\s*))+)?", RegexOptions.Compiled).ToFactory<Node>();

        private readonly List<Node> fChildren = [];

        public string Name { get; }
        public int Weight { get; }
        public ImmutableArray<string> ChildNames { get; }
        public Node? Parent { get; private set; }

        private Node(string name, int weight, string[] childNames)
        {
            Name = name;
            Weight = weight;
            ChildNames = childNames != null ? [.. childNames] : [];
        }

        public void AddChild(Node child)
        {
            fChildren.Add(child);
            child.Parent = this;
        }

        public int TotalWeight => Weight + fChildren.Sum(c => c.TotalWeight);

        public (Node?, int) GetUnbalancedNode()
        {
            var groups = fChildren.GroupBy(c => c.TotalWeight).OrderBy(g => g.Count()).ToList();
            var targetWeight = groups.Last().Key;
            if (groups.Count > 1)
                return (groups.First().First(), targetWeight);
            return (null, targetWeight);
        }
    }

    private static Node GetRoot(string input)
    {
        var nodes = input.Lines().Select(Node.Parse).NonNull().ToDictionary(n => n.Name);

        foreach (var parent in nodes.Values)
            foreach (var childName in parent.ChildNames)
                parent.AddChild(nodes[childName]);
        var root = nodes.Values.First();
        while (root.Parent != null)
            root = root.Parent;
        return root;
    }

    private static int GetFixedWeight(string input)
    {
        (Node child, int targetWeight) = (GetRoot(input), 0);
        while (child != null)
        {
            (var unbalanced, var weight) = child.GetUnbalancedNode();
            if (unbalanced == null)
                return child.Weight + (targetWeight - child.TotalWeight);
            else
                (child, targetWeight) = (unbalanced, weight);
        }
        return -1;
    }

    protected override string Part1()
    {
        Assert(GetRoot(Sample()).Name, "tknk");
        return GetRoot(Input).Name;
    }

    protected override int Part2()
    {
        Assert(GetFixedWeight(Sample()), 60);
        return GetFixedWeight(Input);
    }
}
