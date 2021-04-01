using AdventOfCode.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2018
{
    class Day20 : Solution
    {
        enum Direction
        {
            North = 0b00,
            South = 0b01,
            West = 0b10,
            East  = 0b11,
        }

        private static Point2D[] Offsets = new[]
        {
            new Point2D(0, -1),
            new Point2D(0, +1),
            new Point2D(-1, 0),
            new Point2D(+1, 0),
        };

       
        private IEnumerable<(Point2D From, Point2D To)> Edges(string regex)
        {
            var p = Point2D.Origin;
            var groups = new Stack<Point2D>();
            
            foreach(var c in regex)
            {
                var o = p;
                switch(c)
                {
                    case 'N': p -= (0, 1); break;
                    case 'S': p += (0, 1); break;
                    case 'W': p -= (1, 0); break;
                    case 'E': p += (1, 0); break;
                    case '(': groups.Push(p); continue;
                    case '|': p = groups.Peek(); continue;
                    case ')': p = groups.Pop(); continue;
                    default: continue;
                }
                yield return (p, o);
                yield return (o, p);
            }
        }

        private IEnumerable<int> Distances(string regex)
        {
            var edges = Edges(regex).ToLookup(p => p.From, p => p.To);
            var open = new Queue<Point2D>();
            var distances = new Dictionary<Point2D, int>();

            void Visit(Point2D room, int dist) 
            { 
                if (distances.TryAdd(room, dist)) 
                    open.Enqueue(room); 
            }

            Visit(Point2D.Origin, 0);
            while (open.TryDequeue(out var p))
            {
                var d = distances[p]+1;
                foreach (var n in edges[p])
                    Visit(n, d);
            }
            return distances.Values;
        }

        protected override long? Part1()
        {
            Assert(Distances("^WNE$").Max(), 3);
            Assert(Distances("^ENWWW(NEEE|SSE(EE|N))$").Max(), 10);
            Assert(Distances("^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$").Max(), 18);
            Assert(Distances("^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$").Max(), 23);
            Assert(Distances("^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$").Max(), 31);

            return Distances(Input).Max();
        }

        protected override long? Part2() => Distances(Input).Where(n => n >= 1000).Count();

    }
}
