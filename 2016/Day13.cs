namespace AdventOfCode._2016;

using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day13 : Solution
{
    private static int NumberOfSetBits(ulong i)
    {
        i -= ((i >> 1) & 0x5555555555555555UL);
        i = (i & 0x3333333333333333UL) + ((i >> 2) & 0x3333333333333333UL);
        return (int)(unchecked(((i + (i >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
    }
    private static bool IsWall(Point2D pt, int wallPattern)
    {
        long tileNo = (pt.X * pt.X) + (3 * pt.X) + (2 * pt.X * pt.Y) + pt.Y + (pt.Y * pt.Y);
        tileNo += wallPattern;
        return (NumberOfSetBits((ulong)tileNo) % 2) != 0;
    }

    private static IEnumerable<Point2D> Neighbours(Point2D pt, int wallPattern)
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

    class WallAStar : AStar<Point2D>
    {
        private readonly int fWallPattern;
        public WallAStar(Point2D src, int wallPattern)
            : base(src)
        {
            fWallPattern = wallPattern;
        }
        protected override long Distance(Point2D one, Point2D another) => one.ManhattanDistance(another);

        protected override IEnumerable<Point2D> NeighboursOf(Point2D node) => Neighbours(node, fWallPattern);
    }

    private static long ShortestPath(Point2D src, Point2D dest, int wallPattern)
        => new WallAStar(src, wallPattern).ShortestPath(dest).Cost;

    protected override long? Part1()
    {
        Assert(ShortestPath((1, 1), (7, 4), 10), 11);
        return ShortestPath((1, 1), (31, 39), int.Parse(Input));
    }

    private static long CountReachableTiles(Point2D src, int wallPattern, int maxMoves)
    {
        var visited = new HashSet<Point2D> { src };
        var open = new Queue<(Point2D pt, int moves)>();
        open.Enqueue((src, 0));
        while (open.TryDequeue(out var cur))
        {
            var nextCost = cur.moves + 1;
            if (nextCost <= maxMoves)
            {
                foreach (var n in Neighbours(cur.pt, wallPattern))
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
        return CountReachableTiles((1, 1), int.Parse(Input), 50);
    }
}
