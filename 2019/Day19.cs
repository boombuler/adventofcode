namespace AdventOfCode._2019;

class Day19 : Solution
{
    private IntCodeVM fVM;
    private IntCodeVM VM => fVM ??= new IntCodeVM(Input);

    private bool IsInBeam(Point2D<int> pt) => VM.Run(pt.X, pt.Y).First() == 1;

    protected override long? Part1()
        => Point2D<int>.Range((0, 0), (49, 49)).Count(IsInBeam);

    protected override long? Part2()
    {
        const int SIZE = 100 - 1;
        int x = 0;
        for (int y = SIZE; true; y++)
        {
            while (!IsInBeam((x, y)))
                x++;
            var topLeft = new Point2D<int>(x, y - SIZE);
            if (IsInBeam(topLeft) && IsInBeam(topLeft + (SIZE, 0)) && IsInBeam(topLeft + (SIZE, SIZE)))
                return (topLeft.X * 10000) + topLeft.Y;
        }
    }
}
