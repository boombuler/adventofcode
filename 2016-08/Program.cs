using AdventHelper;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2016_08
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private void PrintScreen(bool[,] screen)
        {
            for (int r = 0; r < screen.GetLength(0); r++)
            {
                Debug(new string(Enumerable.Range(0, screen.GetLength(1)).Select(c => screen[r, c] ? '#' : ' ').ToArray()));
            }
            Debug(null);
        }

        class Rect { public int w; public int h; }
        class Rotate { public string direction; public int idx; public int offset; }
        private static readonly Regex ParseRect = new Regex(@"rect (?<w>\d+)x(?<h>\d+)");
        private static readonly Regex ParseRotate = new Regex(@"rotate (?<direction>column|row) (x|y)=(?<idx>\d+) by (?<offset>\d+)");

        private bool[,] RunCode(string commandFile, int screenWidth, int screenHeight)
        {
            var screen = new bool[screenHeight, screenWidth];
            foreach (var cmd in ReadLines(commandFile))
            {
                if (ParseRect.TryMatch(cmd, out Rect rect))
                {
                    for (int x = 0; x < rect.w; x++)
                        for (int y = 0; y < rect.h; y++)
                            screen[y, x] = true;
                }
                else if (ParseRotate.TryMatch(cmd, out Rotate rot))
                {
                    for (int o = 0; o < rot.offset; o++)
                    {
                        if (rot.direction == "column")
                        {
                            var x = rot.idx;
                            var last = screen[screenHeight - 1, x];
                            for (int y = screenHeight - 1; y > 0; y--)
                                screen[y, x] = screen[y - 1, x];
                            screen[0, x] = last;
                        }
                        else
                        {
                            var y = rot.idx;
                            var last = screen[y, screenWidth - 1];
                            for (int x = screenWidth - 1; x > 0; x--)
                                screen[y, x] = screen[y, x - 1];
                            screen[y, 0] = last;
                        }
                    }
                }
                else Error("Unknown Command " + cmd);
            }
            return screen;
        }


        protected override long? Part1()
        {
            Func<bool[,], long> Count = s => s.Cast<bool>().Where(b => b).Count();
            Assert(Count(RunCode("Sample", 7, 3)), 6);

            return Count(RunCode("Input", 50, 6));
        }

        protected override long? Part2()
        {
            PrintScreen(RunCode("Input", 50, 6));
            return base.Part2();
        }
    }
}
