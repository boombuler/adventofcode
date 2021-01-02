using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using AdventHelper;

namespace _2016_22
{
    class Program : ProgramBase
    {
        class Node
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Used { get; set; }
            public int Avail { get; set; }
            public int Size { get; set; }
        }

        static void Main(string[] args) => new Program().Run();

        private static readonly Func<string, Node> NodeFactory
            = new Regex(@"/dev/grid/node-x(?<X>\d+)-y(?<Y>\d+)\s*(?<Size>\d+)T\s*(?<Used>\d+)T\s*(?<Avail>\d+)T\s*(?<UsePerc>\d+)%", RegexOptions.Compiled).ToFactory<Node>();

        private int GetViablePairCount(string df)
        {
            var nodes = ReadLines(df).Skip(2).Select(NodeFactory).ToList();
            return nodes.Where(na => na.Used > 0)
                .Sum(na => nodes.Where(nb => nb != na && nb.Avail >= na.Used).Count());
        }

        protected override long? Part1() => GetViablePairCount("Input");

        private int FindShortestPath(string df, bool debug = false)
        {
            var nodes = ReadLines(df).Skip(2).Select(NodeFactory).ToList();
            var minSize = nodes.Min(n => n.Size);
            var maxX = nodes.Max(n => n.X);

            Node wallStart = null;
            Node empty = null;

            foreach (var grp in nodes.GroupBy(n => n.Y).OrderBy(g => g.Key))
            {
                var s = "";
                foreach(var node in grp.OrderBy(i => i.X))
                {
                    if (node.Used > minSize)
                    {
                        s += "#";
                        wallStart ??= node;
                    }
                    else if (node.Used == 0)
                    {
                        s += "_";
                        empty = node;
                    }
                    else if (node.X == maxX && node.Y == 0)
                        s += "G";
                    else 
                        s += ".";
                }
                if (debug)
                    Debug(s);
            }


            return (empty.X - (wallStart.X - 1)) // Move left of wall
                + empty.Y                        // Move to top row
                + (maxX - wallStart.X + 1)       // Move to the right
                + (5 * (maxX - 1));              // Swap the data to left (takes 5 turns each time)

        }

        protected override long? Part2() => FindShortestPath("Input");
    }
}
