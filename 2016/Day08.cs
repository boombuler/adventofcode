namespace AdventOfCode._2016;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day08 : Solution<long?, string>
{
    record Rect(int Width, int Height);
    record Rotate(string Direction, int Index, int Offset);
    private static readonly Regex ParseRect = new(@"rect (?<Width>\d+)x(?<Height>\d+)");
    private static readonly Regex ParseRotate = new(@"rotate (?<Direction>column|row) (x|y)=(?<Index>\d+) by (?<Offset>\d+)");

    private bool[,] RunCode(string commands, int screenWidth, int screenHeight)
    {
        var screen = new bool[screenHeight, screenWidth];
        foreach (var cmd in commands.Lines())
        {
            if (ParseRect.TryMatch(cmd, out Rect rect))
            {
                for (int x = 0; x < rect.Width; x++)
                    for (int y = 0; y < rect.Height; y++)
                        screen[y, x] = true;
            }
            else if (ParseRotate.TryMatch(cmd, out Rotate rot))
            {
                for (int o = 0; o < rot.Offset; o++)
                {
                    if (rot.Direction == "column")
                    {
                        var x = rot.Index;
                        var last = screen[screenHeight - 1, x];
                        for (int y = screenHeight - 1; y > 0; y--)
                            screen[y, x] = screen[y - 1, x];
                        screen[0, x] = last;
                    }
                    else
                    {
                        var y = rot.Index;
                        var last = screen[y, screenWidth - 1];
                        for (int x = screenWidth - 1; x > 0; x--)
                            screen[y, x] = screen[y, x - 1];
                        screen[y, 0] = last;
                    }
                }
            }
            else 
                Error("Unknown Command " + cmd);
        }
        return screen;
    }

    protected override long? Part1()
    {
        static long Count(bool[,] s) => s.Cast<bool>().Where(b => b).Count();
        Assert(Count(RunCode(Sample(), 7, 3)), 6);

        return Count(RunCode(Input, 50, 6));
    }

    protected override string Part2()
    {
        var screen = RunCode(Input, 50, 6);
        var ocr = new OCR6x5();
        return ocr.Decode((x, y) => screen[y, x], screen.GetLength(1));
    }
}
