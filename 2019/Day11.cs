using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode._2019
{
    class Day11 : Solution<long, string>
    {
        enum TurnDirection
        {
            Left = 0,
            Right = 1
        }

        class Robot
        {
            private static Point2D[] MoveDirections = { ( 0, -1 ), (1, 0), (0, 1), (-1,0) };
            private IntCodeVM fVM;
            private Point2D fPosition;
            private int fDirection = 0;
            
            private HashSet<Point2D> fWhiteTiles = new HashSet<Point2D>();
            private HashSet<Point2D> fPaintedTiles = new HashSet<Point2D>();
            
            public Robot(string program)
            {
                fVM = new IntCodeVM(program);
            }

            public long Run(bool startOnWhiteTile)
            {
                fPosition = Point2D.Origin;
                if (startOnWhiteTile)
                    fWhiteTiles.Add(fPosition);

                var commands = fVM.Run(new LazyIntCodeVMParam(GetCameraValue)).GetEnumerator();

                while(commands.NextValue(out long color))
                {
                    Paint(color);
                    if (!commands.NextValue(out long dir))
                        break;
                    Turn(dir);
                    fPosition += MoveDirections[fDirection];
                }
                return fPaintedTiles.Count;
            }

            private void Paint(long color)
            {
                fPaintedTiles.Add(fPosition);
                if (color == 1)
                    fWhiteTiles.Add(fPosition);
                else
                    fWhiteTiles.Remove(fPosition);
            }

            public bool IsPainted(Point2D p) => fWhiteTiles.Contains(p);

            private void Turn(long turnDirection)
            {
                fDirection += (turnDirection == 0) ? 3 : 1;
                fDirection %= MoveDirections.Length;
            }
            private long GetCameraValue() => fWhiteTiles.Contains(fPosition) ? 1 : 0;
        }

        protected override long Part1()
            =>  new Robot(Input).Run(false);

        protected override string Part2()
        {
            var robot = new Robot(Input);
            robot.Run(true);
            return new OCR6x5().Decode((x, y) => robot.IsPainted((x+1, y)), 40);
        }
    }
}
