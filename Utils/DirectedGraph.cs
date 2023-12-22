namespace AdventOfCode.Utils;

using System.Collections;

public class DirectedGraph<T>
{
    readonly struct GroupingWrapper(T key, HashSet<T> values) : IGrouping<T, T>
    {
        public T Key => key;
        public IEnumerator<T> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class LookupWrapper(Dictionary<T, HashSet<T>> dictionary) : ILookup<T, T>
    {
        public IEnumerable<T> this[T key] => dictionary.GetValueOrDefault(key) ?? Enumerable.Empty<T>();

        public int Count => dictionary.Values.Sum(t => t.Count);

        public bool Contains(T key) => dictionary.ContainsKey(key);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IGrouping<T, T>> GetEnumerator()
        {
            foreach(var kvp in dictionary)
            {
                yield return new GroupingWrapper(kvp.Key, kvp.Value);
            }
        }
    }

    private readonly Dictionary<T, HashSet<T>> fIncoming = [];
    private readonly Dictionary<T, HashSet<T>> fOutgoing = [];
    private static HashSet<T> NewSet() => [];

    public IEnumerable<T> Sinks => fIncoming.Keys.Where(i => !fOutgoing.ContainsKey(i));
    public IEnumerable<T> Sources => fOutgoing.Keys.Where(i => !fIncoming.ContainsKey(i));
    public IEnumerable<T> Vertices => fIncoming.Keys.Union(fOutgoing.Keys); 

    public ILookup<T, T> Outgoing { get; }
    public ILookup<T, T> Incoming { get; } 

    public DirectedGraph()
    {
        Outgoing = new LookupWrapper(fOutgoing);
        Incoming = new LookupWrapper(fIncoming);
    }

    public void Add(T from, T to)
    {
        fIncoming.GetOrAdd(to, NewSet).Add(from);
        fOutgoing.GetOrAdd(from, NewSet).Add(to);
    }

    public void AddRange(IEnumerable<T> from, T to)
        => from.ForEach(f => Add(f, to));
    public void AddRange(T from, IEnumerable<T> to)
        => to.ForEach(t => Add(from, t));

    public bool Contains(T vertex)
        => fIncoming.ContainsKey(vertex) || fOutgoing.ContainsKey(vertex);
}
