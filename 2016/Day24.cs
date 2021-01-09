using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2016
{
    class Day24 : Solution
    {
        class Node
        {
            public int? Number { get; }
            public Point2D Location { get; }
            public List<Node> Neigbours { get; } = new List<Node>();

            public Node(char c, Point2D l)
            {
                Location = l;
                Number = c != '.' && c != '#' ? c - '0' : (int?)null;
            }
        }

        class NodeAStar : AStar<Node>
        {
            public NodeAStar(Node src) 
                : base(src)
            {
            }

            protected override long Distance(Node one, Node another) => one.Location.ManhattanDistance(another.Location);

            protected override IEnumerable<Node> NeighboursOf(Node node) => node.Neigbours;
        }

        private Dictionary<int, Node> ReadGrid(string map)
        {
            var cells = map.Lines()
                .SelectMany((line, y) => line.Select((c, x) => c == '#' ? null : new Node(c, (x, y))))
                .Where(c => c != null)
                .ToDictionary(c => c.Location);
            var result = new Dictionary<int, Node>();
            foreach(var kvp in cells)
            {
                foreach(var n in kvp.Key.Neighbours())
                {
                    if (cells.TryGetValue(n, out var other))
                        kvp.Value.Neigbours.Add(other);
                }
                if (kvp.Value.Number.HasValue)
                    result[kvp.Value.Number.Value] = kvp.Value;
            }
            return result;
        }

        public long GetShortestPath(string map, bool returnToZero = false)
        {
            var grid = ReadGrid(map);
            Dictionary<(int, int), long> distances = new Dictionary<(int, int), long>();
            foreach(var src in grid.Keys)
            {
                var astar = new NodeAStar(grid[src]);
                foreach(var dest in grid.Keys)
                {
                    if (dest == src)
                        continue;

                    distances[(src, dest)] = astar.ShortestPath(grid[dest]);
                }
            }

            var minCost = long.MaxValue;
            foreach(var travlePath in grid.Keys.Where(k => k != 0).Permuatate())
            {
                int last = 0;
                long cost = 0;
                foreach(var n in travlePath)
                {
                    cost += distances[(last, n)];
                    last = n;
                }
                if (returnToZero)
                    cost += distances[(last, 0)];

                minCost = Math.Min(minCost, cost);
            }
            return minCost;
        }

        protected override long? Part1()
        {
            Assert(GetShortestPath(Sample()), 14);
            return GetShortestPath(Input);
        }

        protected override long? Part2() => GetShortestPath(Input, true);

    }
}
