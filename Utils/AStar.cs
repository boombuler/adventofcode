using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Utils
{
    public abstract class AStar<T>
    {
        protected abstract long Distance(T one, T another);
        
        private readonly T fSrc;
        private readonly Dictionary<T, long> Costs = new Dictionary<T, long>();
        private readonly Dictionary<T, T> Paths = new Dictionary<T, T>();

        public AStar(T src)
            => fSrc = src;

        protected abstract IEnumerable<T> NeighboursOf(T node);

        public long ShortestPath(T dest)
        {
            long GuessedCost(T other)
            {
                var dist = Distance(other, dest);
                if (Paths.TryGetValue(other, out var par))
                    return Costs[par] + dist;
                return dist;
            }

            var comparer = Comparer<T>.Create((a, b) => Math.Sign(GuessedCost(a) - GuessedCost(b)));
            var open = new HashSet<T>();
            var closed = new HashSet<T>();

            open.Add(fSrc);
            Costs[fSrc] = 0;

            while (open.Count > 0)
            {
                var current = open.Min(comparer);

                if (Equals(current, dest))
                    return Costs[current];

                open.Remove(current);
                closed.Add(current);

                foreach (var successor in NeighboursOf(current))
                {
                    if (closed.Contains(successor))
                        continue;
                    open.Add(successor);
                    var cost = Costs[current] + Distance(current, successor);
                    if (!Costs.TryGetValue(successor, out long oldCosts) || oldCosts > cost)
                    {
                        Costs[successor] = cost;
                        Paths[successor] = current;
                    }             
                }
            }
            return -1;
        }

    }
}
