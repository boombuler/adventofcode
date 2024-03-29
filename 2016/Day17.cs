﻿namespace AdventOfCode._2016;

using System.Security.Cryptography;
using Point = Point2D<int>;
class Day17 : Solution<string, long?>
{
    private static readonly Point Target = new(3, 3);
    private static readonly Point Start = new(0, 0);

    class State
    {
        private readonly Point fPosition;
        private readonly string fSeed;
        public string Directions { get; }
        public bool Won => fPosition.Equals(Target);

        public State(string seed)
            : this(seed, Start, string.Empty)
        {
        }

        private State(string seed, Point pos, string dir)
        {
            fSeed = seed;
            fPosition = pos;
            Directions = dir;
        }

        public IEnumerable<State> ValidMoves()
        {
            Span<byte> hash = stackalloc byte[MD5.HashSizeInBytes];
            MD5.HashData(Encoding.ASCII.GetBytes(fSeed + Directions), hash);
            // up, down, left, and right
            var doorStates = new[]
            {
                (hash[0] >> 4) > 0x0A,
                (hash[0] & 0x0F) > 0x0A,
                (hash[1] >> 4) > 0x0A,
                (hash[1] & 0x0F) > 0x0A,
            };

            if (fPosition.Y > 0 && doorStates[0]) // Up
                yield return new State(fSeed, fPosition + Point.Up, Directions + "U");
            if (fPosition.Y < 3 && doorStates[1]) // Down
                yield return new State(fSeed, fPosition + Point.Down, Directions + "D");
            if (fPosition.X > 0 && doorStates[2]) // Left
                yield return new State(fSeed, fPosition + Point.Left, Directions + "L");
            if (fPosition.X < 3 && doorStates[3]) // Right
                yield return new State(fSeed, fPosition + Point.Right, Directions + "R");
        }
    }

    private static IEnumerable<string> FindAllPaths(string seed)
    {
        var open = new Queue<State>();
        open.Enqueue(new State(seed));

        while (open.TryDequeue(out State state))
        {
            if (state.Won)
                yield return state.Directions;
            else
                foreach (var s in state.ValidMoves())
                    open.Enqueue(s);
        }
    }

    private static string FindShortestPath(string seed)
        => FindAllPaths(seed).FirstOrDefault();

    private static long FindLongestPathLength(string seed)
        => (FindAllPaths(seed).OrderByDescending(p => p.Length).FirstOrDefault() ?? string.Empty).Length;

    protected override string Part1()
    {
        Assert(FindShortestPath("hijkl"), null);
        Assert(FindShortestPath("ihgpwlah"), "DDRRRD");
        Assert(FindShortestPath("kglvqrro"), "DDUDRLRRUDRD");
        Assert(FindShortestPath("ulqzkmiv"), "DRURDRUDDLLDLUURRDULRLDUUDDDRR");
        return FindShortestPath(Input);
    }

    protected override long? Part2()
    {
        Assert(FindLongestPathLength("ihgpwlah"), 370);
        Assert(FindLongestPathLength("kglvqrro"), 492);
        Assert(FindLongestPathLength("ulqzkmiv"), 830);
        return FindLongestPathLength(Input);
    }
}
