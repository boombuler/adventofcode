namespace AdventOfCode.Utils;

using System.Collections;

public class DirectedGraph<TNode, TValue>
{
    readonly struct GroupingWrapper(TNode key, IEnumerable<TNode> values) : IGrouping<TNode, TNode>
    {
        public TNode Key => key;
        public IEnumerator<TNode> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class LookupWrapper(Dictionary<TNode, Dictionary<TNode, TValue>> dictionary) : ILookup<TNode, TNode>
    {
        public IEnumerable<TNode> this[TNode key] => dictionary.GetValueOrDefault(key)?.Keys ?? Enumerable.Empty<TNode>();

        public int Count => dictionary.Values.Sum(t => t.Count);

        public bool Contains(TNode key) => dictionary.ContainsKey(key);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IGrouping<TNode, TNode>> GetEnumerator()
        {
            foreach(var kvp in dictionary)
            {
                yield return new GroupingWrapper(kvp.Key, kvp.Value.Keys);
            }
        }
    }

    private readonly Dictionary<TNode, Dictionary<TNode, TValue>> fIncoming = [];
    private readonly Dictionary<TNode, Dictionary<TNode, TValue>> fOutgoing = [];
    private static Dictionary<TNode, TValue> NewSet() => [];

    public IEnumerable<TNode> Sinks => fIncoming.Keys.Where(i => !fOutgoing.ContainsKey(i));
    public IEnumerable<TNode> Sources => fOutgoing.Keys.Where(i => !fIncoming.ContainsKey(i));
    public IEnumerable<TNode> Vertices => fIncoming.Keys.Union(fOutgoing.Keys); 

    public ILookup<TNode, TNode> Outgoing { get; }
    public ILookup<TNode, TNode> Incoming { get; } 

    public TValue this[TNode from, TNode to] => fOutgoing[from][to];

    public DirectedGraph()
    {
        Outgoing = new LookupWrapper(fOutgoing);
        Incoming = new LookupWrapper(fIncoming);
    }

    public void Add(TNode from, TNode to, TValue value)
    {
        fIncoming.GetOrAdd(to, NewSet)[from] = value;
        fOutgoing.GetOrAdd(from, NewSet)[to] = value;
    }

    public void AddRange(IEnumerable<TNode> from, TNode to, TValue value)
        => from.ForEach(f => Add(f, to, value));
    public void AddRange(TNode from, IEnumerable<TNode> to, TValue value)
        => to.ForEach(t => Add(from, t, value));

    public bool Contains(TNode vertex)
        => fIncoming.ContainsKey(vertex) || fOutgoing.ContainsKey(vertex);
}
