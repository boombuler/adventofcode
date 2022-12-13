namespace AdventOfCode._2022;

using System.Diagnostics;

class Day13 : Solution
{
    class Packet : IComparable<Packet>
    {
        public int? Value { get; }
        public ImmutableList<Packet> Children { get; }

        public Packet(int value) 
            : this(value, ImmutableList<Packet>.Empty) { }
        public Packet(ImmutableList<Packet> children) 
            : this(null, children) { }
        private Packet(int? value, ImmutableList<Packet> children)
        { 
            Value = value;
            Children = children;
        }
        
        public int CompareTo(Packet node)
        {
            if (Value.HasValue && node.Value.HasValue)
                return this.Value.Value - node.Value.Value;

            var self = this.Value.HasValue ? ImmutableList<Packet>.Empty.Add(this) : this.Children;
            var other = node.Value.HasValue ? ImmutableList<Packet>.Empty.Add(node) : node.Children;

            return self.Zip(other)
                .Select((n) => n.First.CompareTo(n.Second))
                .FirstOrDefault(c => c != 0, self.Count - other.Count);
        }
   
        public static Packet Parse(string text)
        {
            Packet ReadNum(StringReader sr)
                => sr.TryReadWhile(char.IsDigit, out var n) ? new Packet(int.Parse(n)) : null;

            Packet ReadList(StringReader sr)
            {
                var items = ImmutableList<Packet>.Empty;
                sr.Read();// Skip leading [
                while (true)
                {
                    switch(sr.Peek())
                    {
                        case ',': sr.Read(); break;
                        case ']':
                            sr.Read(); 
                            return new Packet(items);
                        case '[':
                            items = items.Add(ReadList(sr)); break;
                        default:
                            items = items.Add(ReadNum(sr)); break;
                    }
                }
            }
            using var sr = new StringReader(text);
            return ReadList(sr);
        }
    }

    private IEnumerable<Packet> ReadPackets(string input)
        => input.Lines().Where(x => !string.IsNullOrEmpty(x)).Select(Packet.Parse);

    private long SumIndicesOfSortedPairs(string input)
        => ReadPackets(input).Chunk(2)
            .Select((p, i) => (Ordered: p[0].CompareTo(p[1]) < 0, Idx: i + 1))
            .Where(n => n.Ordered).Sum(n => n.Idx);

    private long GetDividerIndices(string input)
    {
        var dividers = new [] { Packet.Parse("[[2]]"),  Packet.Parse("[[6]]") };
        var nodes = ReadPackets(input).Concat(dividers).OrderBy(Functional.Identity).ToList();
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
