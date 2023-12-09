namespace AdventOfCode._2015;

class Day03 : Solution
{
    public static IEnumerable<Point2D> Walk(IEnumerable<char> directions)
        => directions.Select(Point2D.DirectionFromArrow).Scan(Point2D.Origin, (loc, dir) => loc + dir);

    public static int CountUniquePlaces(string directions, int workers = 1)
        => Enumerable.Range(0, workers)
            .SelectMany(w => Walk(directions.Skip(w).Where((c, i) => (i % workers) == 0)))
            .Distinct()
            .Count();

    protected override long? Part1()
    {
        Assert(CountUniquePlaces(">"), 2);
        Assert(CountUniquePlaces("^>v<"), 4);
        Assert(CountUniquePlaces("^v^v^v^v^v"), 2);
        return CountUniquePlaces(Input);
    }

    protected override long? Part2()
    {
        Assert(CountUniquePlaces("^v", 2), 3);
        Assert(CountUniquePlaces("^>v<", 2), 3);
        Assert(CountUniquePlaces("^v^v^v^v^v", 2), 11);
        return CountUniquePlaces(Input, 2);
    }
}
