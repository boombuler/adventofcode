namespace AdventOfCode.Utils;

public abstract class AStar<T>(T src) where T : notnull
{
    protected abstract long Distance(T from, T to);
    protected virtual long GuessDistance(T from, T to)
        => Distance(from, to);

    protected abstract IEnumerable<T> NeighboursOf(T node);

    protected virtual bool AreEqual(T one, T another)
        => Equals(one, another);

    public (long Cost, Lazy<IEnumerable<T>> Path) ShortestPath(T dest)
    {
        var open = new PriorityQueue<T, long>();
        open.Enqueue(src, 0);
        var costs = new Dictionary<T, long>() { [src] = 0 };
        var paths = new Dictionary<T, T> { [src] = src };

        while (open.Count > 0)
        {
            var current = open.Dequeue();
            var curCost = costs[current];
            if (AreEqual(current, dest))
                return (curCost, new Lazy<IEnumerable<T>>(() => BuildPath(dest, paths)));

            foreach (var next in NeighboursOf(current))
            {
                var newCost = curCost + Distance(current, next);
                if (!costs.TryGetValue(next, out var oldCost) || newCost < oldCost)
                {
                    costs[next] = newCost;
                    var priority = newCost + GuessDistance(next, dest);
                    open.Enqueue(next, priority);
                    paths[next] = current;
                }
            }
        }
        return (-1, new Lazy<IEnumerable<T>>(Enumerable.Empty<T>));
    }

    private IEnumerable<T> BuildPath(T dest, Dictionary<T, T> shortestPath)
    {
        dest = shortestPath.Keys.First(k => AreEqual(k, dest));
        var s = new Stack<T>();
        do
        {
            s.Push(dest);
            dest = shortestPath[dest];
        } while (!AreEqual(dest, src));
        return s;
    }
}
