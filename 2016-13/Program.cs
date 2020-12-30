using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2016_13
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();
        static int NumberOfSetBits(ulong i)
        {
            i = i - ((i >> 1) & 0x5555555555555555UL);
            i = (i & 0x3333333333333333UL) + ((i >> 2) & 0x3333333333333333UL);
            return (int)(unchecked(((i + (i >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }
        private bool IsWall(Point2D pt, int wallPattern)
        {
            long tileNo = (pt.X * pt.X) + (3 * pt.X) + (2 * pt.X * pt.Y) + pt.Y + (pt.Y * pt.Y);
            tileNo += wallPattern;
            return (NumberOfSetBits((ulong)tileNo) % 2) != 0;
        }

        private IEnumerable<Point2D> Neighbours(Point2D pt, int wallPattern)
        {
            bool isValid(Point2D p) => p.X >= 0 && p.Y >= 0 && !IsWall(p, wallPattern);

            return new Point2D[]
            {
                pt - (0, 1),
                pt + (0, 1),
                pt - (1, 0),
                pt + (1, 0),
            }.Where(isValid);
        }

        private long ShortestPath(Point2D src, Point2D dest, int wallPattern)
        {
            var parents = new Dictionary<Point2D, Point2D>();
            var costs = new Dictionary<Point2D, int>();

            long GuessedCost(Point2D pt) 
            {
                var dist = (dest - pt);
                var cost = Math.Abs(dist.X) + Math.Abs(dist.Y);
                if (parents.TryGetValue(pt, out var par))
                    return costs[par] + cost;
                return cost;
            }
            var comparer = Comparer<Point2D>.Create((a, b) => Math.Sign(GuessedCost(a) - GuessedCost(b)));


            var open = new HashSet<Point2D>();
            var closed = new HashSet<Point2D>();
            
            open.Add(src);
            costs[src] = 0;

            while (open.Count > 0)
            {
                var current = open.OrderBy(x => x, comparer).First();
                
                if (Equals(current, dest))
                    return costs[current];
                
                open.Remove(current);
                closed.Add(current);

                var childCost = costs[current] + 1;
                foreach (var successor in Neighbours(current, wallPattern))
                {
                    if (closed.Contains(successor))
                        continue;
                    if (!open.Contains(successor))
                    {
                        costs[successor] = childCost;
                        parents[successor] = current;
                        open.Add(successor);
                    }
                    else if (costs[successor] > childCost)
                    {
                        costs[successor] = childCost;
                        parents[successor] = current;
                    }
                }
            }
            return -1;
        }

        protected override long? Part1()
        {
            Assert(ShortestPath((1, 1), (7, 4), 10), 11);
            return ShortestPath((1, 1), (31, 39), 1364);
        }

        private long CountReachableTiles(Point2D src, int wallPattern, int maxMoves)
        {
            var visited = new HashSet<Point2D>();
            visited.Add(src);
            var open = new Queue<(Point2D pt, int moves)>();
            open.Enqueue((src, 0));
            while(open.TryDequeue(out var cur))
            {
                var nextCost = cur.moves + 1;
                if (nextCost <= maxMoves)
                {
                    foreach(var n in Neighbours(cur.pt, wallPattern))
                    {
                        if (visited.Add(n)) // if already visited, we found a cheaper route to this point
                            open.Enqueue((n, nextCost)); // and therefor also for its neighbours.
                    }
                }
            }

            return visited.Count;
        }

        protected override long? Part2()
        {
            Assert(CountReachableTiles((1, 1), 10, 0), 1);
            Assert(CountReachableTiles((1, 1), 10, 1), 3);
            Assert(CountReachableTiles((1, 1), 10, 2), 5);
            Assert(CountReachableTiles((1, 1), 10, 3), 6);
            Assert(CountReachableTiles((1, 1), 10, 4), 9);
            return CountReachableTiles((1, 1), 1364, 50);
        }
    }
}
