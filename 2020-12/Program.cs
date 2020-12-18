using AdventHelper;
using System;

namespace _2020_12
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        enum Direction
        {
            E = 0,
            S = 1,
            W = 2,
            N = 3,
            MASK = N
        }

        private long NavigateShip(string inputFile)
        {
            (int y, int x) = (0, 0);
            Direction d = Direction.E;

            foreach(var op in ReadLines(inputFile))
            {
                var value = int.Parse(op.Substring(1));
                var cmd = op.Substring(0, 1);
                switch(cmd)
                {   
                    case "L": d = (d - (value / 90)) & Direction.MASK; break;
                    case "R": d = (d + (value / 90)) & Direction.MASK; break;
                    default:
                        if (!Enum.TryParse<Direction>(cmd, out var dir))
                            dir = d;
                        switch (dir)
                        {
                            case Direction.N: y -= value; break;
                            case Direction.S: y += value; break;
                            case Direction.E: x += value; break;
                            case Direction.W: x -= value; break;
                        }
                        break;
                }
            }
            return Math.Abs(y) + Math.Abs(x);
        }

        private long NavigateWP(string inputFile)
        {
            (int yWP, int xWP) = (-1, 10);
            (int yS, int xS) = (0, 0);
             
            void RotR(int cnt)
            {
                for (int i = 0; i < cnt; i++)
                    (yWP, xWP) = (xWP, -yWP);
            }

            foreach (var op in ReadLines(inputFile))
            {
                var value = int.Parse(op.Substring(1));
                var cmd = op.Substring(0, 1);

                switch (cmd)
                {
                    case "L": RotR(4 - ((value % 360) / 90)); break; 
                    case "R": RotR(value / 90); break;
                    case "F": xS += (value * xWP); yS += (value * yWP); break;
                    default:
                        var dir = Enum.Parse<Direction>(cmd);
                        switch (dir)
                        {
                            case Direction.N: yWP -= value; break;
                            case Direction.S: yWP += value; break;
                            case Direction.E: xWP += value; break;
                            case Direction.W: xWP -= value; break;
                        }
                        break;
                }
            }
            return Math.Abs(yS) + Math.Abs(xS);
        }


        protected override long? Part1()
        {
            Assert(NavigateShip("Sample.txt"), 25);
            return NavigateShip("Input.txt");
        }

        protected override long? Part2()
        {
            Assert(NavigateWP("Sample.txt"), 286);
            return NavigateWP("Input.txt");
        }
    }
}
