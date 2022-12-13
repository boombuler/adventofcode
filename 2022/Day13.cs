namespace AdventOfCode._2022;

using System.Diagnostics;

class Day13 : Solution
{
    class Node : IComparable<Node>
    {
        public int? Value { get; }
        public ImmutableList<Node> Children { get; }

        public Node(int value) 
            : this(value, ImmutableList<Node>.Empty) { }
        public Node(ImmutableList<Node> children) 
            : this(null, children) { }
        private Node(int? value, ImmutableList<Node> children)
        { 
            Value = value;
            Children = children;
        }
        
        public int CompareTo(Node node)
        {
            if (Value.HasValue && node.Value.HasValue)
                return Value.Value.CompareTo(node.Value);

            var self = this.Value.HasValue ? ImmutableList<Node>.Empty.Add(this) : this.Children;
            var other = node.Value.HasValue ? ImmutableList<Node>.Empty.Add(node) : node.Children;

            foreach(var (a,b) in self.Zip(other))
            {
                var cmp = a.CompareTo(b);
                if (cmp != 0) 
                    return cmp;
            }
            return self.Count.CompareTo(other.Count);
        }
   
        public static Node Parse(string text)
        {
            Node ParseFrom(StringReader sr)
            {
                var items = ImmutableList<Node>.Empty;
                while(sr.TryRead(out char c) && c != ']')
                {
                    if (c == ',')
                        continue;
                    if (c == '[')
                        items = items.Add(ParseFrom(sr));
                    else 
                    {
                        var num = new StringBuilder().Append(c);
                        while (sr.TryRead(out var o) && char.IsDigit(o))
                            num.Append(o);
                        items = items.Add(new Node(int.Parse(num.ToString())));
                    } 
                }
                return new Node(items);
            }
            using var sr = new StringReader(text);
            sr.Read(); // Skip leading [
            return ParseFrom(sr);
        }
    }

    private long SumIndicesOfSortedPairs(string input)
        => input.Split("\n\n")
            .Select(grp => grp.Lines().Select(Node.Parse).ToArray())
            .Select((p, i) => (Ordered: p[0].CompareTo(p[1]) < 0, Idx: i + 1))
            .Where(n => n.Ordered).Sum(n => n.Idx);

    private long GetDividerIndices(string input)
    {
        var dividers = new List<Node>()
        {
            Node.Parse("[[2]]"), 
            Node.Parse("[[6]]")
        };
        var nodes = input.Split("\n\n").SelectMany(grp => grp.Lines().Select(Node.Parse)).Concat(dividers)
            .OrderBy(Functional.Identity).ToList();
        return dividers.Select(d => nodes.IndexOf(d) + 1).Aggregate((a, b) => a * b);
    }

    protected override long? Part1()
    {
        Assert(SumIndicesOfSortedPairs(Sample()), 13);
        return SumIndicesOfSortedPairs(Input);
    }

    protected override long? Part2()
    {
        Assert(GetDividerIndices(Sample()), 140);
        return GetDividerIndices(Input);
    }
}
