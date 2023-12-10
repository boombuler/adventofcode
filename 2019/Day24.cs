namespace AdventOfCode._2019;

class Day24 : Solution
{
    private const int SIZE = 5;

    private bool IsBugAlive(bool wasAlive, int activeNeighbours)
        => activeNeighbours == 1 || (!wasAlive && activeNeighbours == 2);

    protected override long? Part1()
    {
        var state = Input.Cells(filter: v => v == '#').Keys.ToHashSet();

        static bool ValidCell(Point2D p) => p.X >= 0 && p.Y >= 0 && p.X < SIZE && p.Y < SIZE;

        var seen = new HashSet<int>();
        while (true)
        {
            GameOfLife.Emulate(state, 1, n => n.Neighbours().Where(ValidCell), IsBugAlive);
            var bd = state.Select(p => p.Y * SIZE + p.X).Sum(bit => 1 << (int)bit);
            if (!seen.Add(bd))
                return bd;
        }
    }

    private IEnumerable<Point3D> GetRecursiveNeighbours(Point3D pt)
    {
        var (pt2d, layer) = pt;
        foreach (var p in pt2d.Neighbours())
            if (p != (2, 2) && p.X >= 0 && p.Y >= 0 && p.X < SIZE && p.Y < SIZE)
                yield return p.WithZ(layer);

        if (pt.X == 0)
            yield return (1, 2, layer - 1);
        if (pt.X == SIZE - 1)
            yield return (3, 2, layer - 1);
        if (pt.Y == 0)
            yield return (2, 1, layer - 1);
        if (pt.Y == SIZE - 1)
            yield return (2, 3, layer - 1);

        var inner = pt2d switch
        {
            (1, 2) => new Func<int, Point2D>(y => new Point2D(0, y)),
            (3, 2) => (y) => (SIZE - 1, y),
            (2, 1) => (x) => (x, 0),
            (2, 3) => (x) => (x, SIZE - 1),
            _ => null
        };
        if (inner != null)
        {
            for (int x = 0; x < SIZE; x++)
                yield return inner(x).WithZ(layer + 1);
        }
    }

    protected override long? Part2()
    {
        var state = Input.Cells(filter: v => v == '#').Select(kvp => kvp.Key.WithZ(0)).ToHashSet();

        return GameOfLife.Emulate(state, 200, GetRecursiveNeighbours, IsBugAlive);
    }
}
