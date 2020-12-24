using AdventHelper;
using System;
using System.Collections.Generic;
using System.IO;

namespace _2020_24
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();
        /*
            [NW][NE]
            [ W][__][ E]
                [SW][SE]
         */
        private IEnumerable<(int r, int c)> GetNeighbours((int r, int c) i)
        {
            yield return (i.r - 1, i.c - 1); // NW
            yield return (i.r - 1, i.c);     // NE
            yield return (i.r, i.c - 1);     // W
            yield return (i.r, i.c + 1);     // E
            yield return (i.r + 1, i.c);     // SW
            yield return (i.r + 1, i.c + 1); // SE
        }

        private long CountBlackTiles(string input) => ReadPattern(input).Count;

        private HashSet<(int r, int c)> ReadPattern(string input)
        {
            var blackTiles = new HashSet<(int r, int c)>();
            foreach(var line in ReadLines(input))
            {
                var pos = (r: 0, c: 0);

                using (var sr = new StringReader(line))
                {
                    while(sr.Peek() > 0)
                    {
                        var c = (char)sr.Read();
                        switch(c)
                        {
                            case 'w': pos = (pos.r, pos.c - 1); break;
                            case 'e': pos = (pos.r, pos.c + 1); break;
                            case 'n':
                                if ((char)sr.Read() == 'w')
                                    pos = (pos.r - 1, pos.c - 1); 
                                else
                                    pos = (pos.r - 1, pos.c);
                                break;
                            case 's':
                                if ((char)sr.Read() == 'w')
                                    pos = (pos.r + 1, pos.c);
                                else
                                    pos = (pos.r + 1, pos.c + 1);
                                break;
                        }
                    }

                    if (!blackTiles.Remove(pos))
                        blackTiles.Add(pos);
                }
            }
            return blackTiles;
        }


        //Any black tile with zero or more than 2 black tiles immediately adjacent to it is flipped to white.
        // Any white tile with exactly 2 black tiles immediately adjacent to it is flipped to black.
        private long GameOfTiles(string input, int days)
            => GameOfLife.Emulate(ReadPattern(input), days, GetNeighbours, (wasAlive, n) => (n == 2) || (wasAlive && n == 1));

        protected override long? Part1()
        {
            Assert(CountBlackTiles("Sample"), 10);
            return CountBlackTiles("Input");
        }

        protected override long? Part2()
        {
            Assert(GameOfTiles("Sample", 1), 15);
            Assert(GameOfTiles("Sample", 2), 12);
            Assert(GameOfTiles("Sample", 20), 132);
            Assert(GameOfTiles("Sample", 100), 2208);
            return GameOfTiles("Input", 100);
        }
    }
}
