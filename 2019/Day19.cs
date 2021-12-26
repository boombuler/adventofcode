namespace AdventOfCode._2019;

using System.Linq;
using AdventOfCode.Utils;

class Day19 : Solution
{
    private IntCodeVM fVM;
    private IntCodeVM VM => fVM ??= new IntCodeVM(Input);

    private bool IsInBeam(Point2D pt) => VM.Run(pt.X, pt.Y).First() == 1;

    protected override long? Part1()
        => Point2D.Range((0, 0), (49, 49)).Count(IsInBeam);

    protected override long? Part2()
    {
        const long SIZE = 100 - 1;
        long x = 0;
        for (long y = SIZE; true; y++)
        {
            while (!IsInBeam((x, y)))
                x++;
            var topLeft = new Point2D(x, y - SIZE);
            if (IsInBeam(topLeft) && IsInBeam(topLeft + (SIZE, 0)) && IsInBeam(topLeft + (SIZE, SIZE)))
                return (topLeft.X * 10000) + topLeft.Y;
        }
    }
}
