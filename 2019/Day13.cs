using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2019
{
    class Day13 : Solution
    {
        enum Tile
        {
            Empty = 0,
            Wall = 1,
            Block = 2,
            HPaddle = 3,
            Ball = 4,
        }

        protected override long? Part1()
        {
            var vm = new IntCodeVM(Input);
            var screen = new Dictionary<Point2D, Tile>();
            
            foreach(var (x, (y, (tile, _))) in vm.Run().Chunk(3))
                screen[(x, y)] = (Tile)tile;

            return screen.Values.Count(t => t == Tile.Block);
        }

        protected override long? Part2()
        {
            var vm = new IntCodeVM(Input).SetAddress(0,2);
            long joyPos = 0;
            long ballPos = 0;
            var joystick = new Func<long>(() => Math.Sign(ballPos - joyPos));

            long score = 0;
            foreach (var (x, (y, (tile, _))) in vm.Run(joystick).Chunk(3))
            {
                if (x == -1 && y == 0)
                    score = tile;
                else switch((Tile)tile)
                {
                    case Tile.Ball: ballPos = x; break;
                    case Tile.HPaddle: joyPos = x; break;
                }
            }
            return score;
        }
    }
}
