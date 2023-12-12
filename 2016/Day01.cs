namespace AdventOfCode._2016;

class Day01 : Solution
{
    private static IEnumerable<Point2D<int>> WalkDirections(string directions)
    {
        int x = 0, y = 0;
        int direction = 0;
        yield return (x, y);

        foreach (var d in directions.Split(",").Select(s => s.Trim()))
        {
            var dist = int.Parse(d[1..]);
            var turn = (d[0] == 'L') ? -1 : 1;
            direction = (direction + turn) % 4;

            var mod = ((direction & 2) == 0) ? -1 : 1;

            for (int i = 0; i < dist; i++)
            {
                if ((direction % 2) == 0)
                    x += mod;
                else
                    y += mod;

                yield return (x, y);
            }
        }
    }

    private static long GetFinalDistance(string directions)
        => WalkDirections(directions).Last().ManhattanDistance(Point2D<int>.Origin);

    private long GetDistanceToFirstPlaceVisitedTwice(string directions)
    {
        var visited = new HashSet<Point2D<int>>();
        foreach (var pt in WalkDirections(directions))
        {
            if (!visited.Add(pt))
                return pt.ManhattanDistance(Point2D<int>.Origin);
        }
        Error("No point visited twice");
        return 0;
    }

    protected override long? Part1()
    {
        Assert(GetFinalDistance("R2, L3"), 5);
        Assert(GetFinalDistance("R2, R2, R2"), 2);
        Assert(GetFinalDistance("R5, L5, R5, R3"), 12);
        return GetFinalDistance(Input);
    }

    protected override long? Part2()
    {
        Assert(GetDistanceToFirstPlaceVisitedTwice("R8, R4, R4, R8"), 4);
        return GetDistanceToFirstPlaceVisitedTwice(Input);
    }
}
