namespace AdventOfCode._2022;

class Day10 : Solution<int, string>
{
    const int ScreenWidth = 40;
    record Signal(int X, int Cycle)
    {
        public int Strength => X * Cycle;
        public Point2D<int> Pos => ((Cycle - 1) % ScreenWidth, (Cycle - 1) / ScreenWidth);
        public bool Draw => Math.Abs(X - Pos.X) <= 1;
    }
    private static IEnumerable<Signal> Signals(string input)
    {
        int x = 1;
        int cycle = 1;
        foreach (var line in input.Lines())
        {
            yield return new Signal(x, cycle++);
            if (line.Split(' ') is ["addx", var n] && int.TryParse(n, out int nVal))
            {
                yield return new Signal(x, cycle++);
                x += nVal;
            }
        }
    }

    protected override int Part1()
    {
        static int Solve(string input)
            => Signals(input).Take(220).Where(s => (s.Cycle + 20) % ScreenWidth == 0).Sum(s => s.Strength);
        Assert(Solve(Sample()), 13140);
        return Solve(Input);
    }

    protected override string Part2()
    {
        var screen = Signals(Input).Where(s => s.Draw).Select(s => s.Pos).ToHashSet();
        return new OCR6x5().Decode((x, y) => screen.Contains((x, y)), ScreenWidth);
    }
}
