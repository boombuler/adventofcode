﻿namespace AdventOfCode.Utils;

public abstract class AStar<T>
{
    protected abstract long Distance(T one, T another);

    private readonly T fSrc;

    protected AStar(T src)
        => fSrc = src;

    protected abstract IEnumerable<T> NeighboursOf(T node);

    public (long Cost, Lazy<IEnumerable<T>> Path) ShortestPath(T dest)
    {
        var open = new PriorityQueue<T, long>();
        open.Enqueue(fSrc, 0);
        var costs = new Dictionary<T, long>() { [fSrc] = 0 };
        var paths = new Dictionary<T, T> { [fSrc] = fSrc };

        while (open.Count > 0)
        {
            var current = open.Dequeue();
            var curCost = costs[current];
            if (Equals(current, dest))
                return (curCost, new Lazy<IEnumerable<T>>(() => BuildPath(dest, paths)));

            foreach (var next in NeighboursOf(current))
            {
                var newCost = curCost + Distance(current, next);
                if (!costs.TryGetValue(next, out var oldCost) || newCost < oldCost)
                {
                    costs[next] = newCost;
                    var priority = newCost + Distance(next, dest);
                    open.Enqueue(next, priority);
                    paths[next] = current;
                }
            }
        }
        return (-1, new Lazy<IEnumerable<T>>(Enumerable.Empty<T>));
    }

    private IEnumerable<T> BuildPath(T dest, Dictionary<T, T> shortestPath)
    {
        var s = new Stack<T>();
        do
        {
            s.Push(dest);
            dest = shortestPath[dest];
        } while (!Equals(dest, fSrc));
        return s;
    }
}
