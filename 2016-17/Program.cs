using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace _2016_17
{
    class Program : ProgramBase<string>
    {
        static void Main(string[] args) => new Program().Run();

        private static readonly Point2D Target = new Point2D(3, 3);
        private static readonly Point2D Start = new Point2D(0, 0);
        
        class State
        {
            private Point2D fPosition;
            private string fSeed;
            public string Directions { get; }
            public bool Won => fPosition.Equals(Target);

            public State(string seed)
                : this (seed, Start, string.Empty)
            {
            }

            private State(string seed, Point2D pos, string dir)
            {
                fSeed = seed;
                fPosition = pos;
                Directions = dir;
            }

            public IEnumerable<State> ValidMoves()
            {
                var hash = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(fSeed + Directions));
                // up, down, left, and right
                var doorStates = new[]
                {
                    (hash[0] >> 4) > 0x0A,
                    (hash[0] & 0x0F) > 0x0A,
                    (hash[1] >> 4) > 0x0A,
                    (hash[1] & 0x0F) > 0x0A,
                };

                if (fPosition.Y > 0 && doorStates[0]) // Up
                    yield return new State(fSeed, fPosition - (0, 1), Directions + "U");
                if (fPosition.Y < 3 && doorStates[1]) // Down
                    yield return new State(fSeed, fPosition + (0, 1), Directions + "D");
                if (fPosition.X > 0 && doorStates[2]) // Left
                    yield return new State(fSeed, fPosition - (1, 0), Directions + "L");
                if (fPosition.X < 3 && doorStates[3]) // Right
                    yield return new State(fSeed, fPosition + (1, 0), Directions + "R");
            }
        }

        private IEnumerable<string> FindAllPaths(string seed)
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
      
        private string FindShortestPath(string seed)
            => FindAllPaths(seed).FirstOrDefault();

        private long FindLongestPathLength(string seed)
            => (FindAllPaths(seed).OrderByDescending(p => p.Length).FirstOrDefault() ?? string.Empty).Length;

        protected override string Part1()
        {
            Assert(FindShortestPath("hijkl"), null);
            Assert(FindShortestPath("ihgpwlah"), "DDRRRD");
            Assert(FindShortestPath("kglvqrro"), "DDUDRLRRUDRD");
            Assert(FindShortestPath("ulqzkmiv"), "DRURDRUDDLLDLUURRDULRLDUUDDDRR");
            return FindShortestPath("hhhxzeay"); 
        }

        protected override string Part2()
        {
            Assert(FindLongestPathLength("ihgpwlah"), 370);
            Assert(FindLongestPathLength("kglvqrro"), 492);
            Assert(FindLongestPathLength("ulqzkmiv"), 830);
            return FindLongestPathLength("hhhxzeay").ToString();
        }
    }
}
