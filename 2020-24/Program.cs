using AdventHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        enum Direction : uint
        {     // Row  Col
            nw = 0xFF_FF, // -1, -1
            ne = 0xFF_00, // -1,  0
            w  = 0x00_FF, //  0, -1
            e  = 0x00_01, //  0,  1
            sw = 0x01_00, //  1,  0
            se = 0x01_01, //  1,  1
        }
        private static (int r, int c) Apply((int r, int c) pt, Direction d)
            => (
                pt.r + ((sbyte)((int)d >> 8)), // Add the upper signed byte to the row
                pt.c + (sbyte)(int)d           // Add the lower signed byte to the column
            );

       
        private long CountBlackTiles(string input) => ReadPattern(input).Count;

        private static readonly Regex ParseLine = new Regex("(nw|ne|sw|se|e|w)", RegexOptions.Compiled);
        private HashSet<(int r, int c)> ReadPattern(string input)
        {
            var blackTiles = new HashSet<(int r, int c)>();
            foreach(var line in ReadLines(input))
            {
                var pos = (r: 0, c: 0);
                foreach(var direction in ParseLine.Matches(line).Select(m => Enum.Parse<Direction>(m.Value)))
                    pos = Apply(pos, direction);
                if (!blackTiles.Remove(pos))
                    blackTiles.Add(pos);
            }
            return blackTiles;
        }

        private long GameOfTiles(string input, int days)
        {
            var allDirections = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray();
            return GameOfLife.Emulate(ReadPattern(input), days,
                i => allDirections.Select(d => Apply(i, d)),
                (wasAlive, n) => (n == 2) || (wasAlive && n == 1));
        }

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
