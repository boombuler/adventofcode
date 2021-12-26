namespace AdventOfCode.Console;

using System;
using Con = System.Console;

abstract class OutputMode
{
    private bool fHasLB = false;
    protected const int DEFAULT_BACKGROUND = 0x0f0f23;
    protected const int DEFAULT_FOREGROUND = 0xffffff;
    protected const int COLOR_CRIMSON = 0xDC143C;

    public virtual void Enter() => Con.WriteLine();

    public virtual void Exit()
    {
        if (!fHasLB)
            Con.WriteLine();
    }

    public virtual void Write(string content)
    {
        Con.Write(content);
        fHasLB = content.EndsWith(Environment.NewLine);

    }
    public virtual void WriteLine(string content)
    {
        Con.WriteLine(content);
        fHasLB = true;
    }

    protected static void SetFG(int color)
        => Con.Write($"\u001b[38;2;{(color >> 16) & 255};{(color >> 8) & 255};{color & 255}m");
    protected static void SetBG(int color)
        => Con.Write($"\u001b[48;2;{(color >> 16) & 255};{(color >> 8) & 255};{color & 255}m");
}
