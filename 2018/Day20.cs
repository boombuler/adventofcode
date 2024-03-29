﻿namespace AdventOfCode._2018;

using Point = Point2D<int>;
class Day20 : Solution
{
    enum Direction
    {
        North = 0b00,
        South = 0b01,
        West = 0b10,
        East = 0b11,
    }

    private static IEnumerable<(Point From, Point To)> Edges(string regex)
    {
        var p = Point.Origin;
        var groups = new Stack<Point>();

        foreach (var c in regex)
        {
            var o = p;
            switch (c)
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

    private static IEnumerable<int> Distances(string regex)
    {
        var edges = Edges(regex).ToLookup(p => p.From, p => p.To);
        var open = new Queue<Point>();
        var distances = new Dictionary<Point, int>();

        void Visit(Point room, int dist)
        {
            if (distances.TryAdd(room, dist))
                open.Enqueue(room);
        }

        Visit(Point.Origin, 0);
        while (open.TryDequeue(out var p))
        {
            var d = distances[p] + 1;
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
