namespace AdventOfCode._2021;

class Day15 : Solution
{
    class PathFinder(Dictionary<Point2D, long> riskMap) : AStar<Point2D>(Point2D.Origin)
    {
        protected override long Distance(Point2D one, Point2D another)
        {
            var distance = one.ManhattanDistance(another);
            if (distance == 1)
                return riskMap[another];
            return distance;
        }

        protected override IEnumerable<Point2D> NeighboursOf(Point2D node)
            => node.Neighbours().Where(riskMap.ContainsKey);
    }

    private static long? FindPath(string input, int expandMap = 1)
    {
        var baseMap = input.Cells(c => c - '0');
        var size = baseMap.Keys.Max() + (1, 1);
        var riskMap = new Dictionary<Point2D, long>();

        foreach (var of in Point2D.Range(Point2D.Origin, (expandMap - 1, expandMap - 1)))
        {
            var offset = new Point2D(of.X * size.X, of.Y * size.Y);
            foreach (var (k, v) in baseMap)
                riskMap[k + offset] = (of.X + of.Y + v - 1) % 9 + 1;
        }

        return new PathFinder(riskMap).ShortestPath(riskMap.Keys.Max()).Cost;
    }

    protected override long? Part1()
    {
        Assert(FindPath(Sample()), 40);
        return FindPath(Input);
    }

    protected override long? Part2()
    {
        Assert(FindPath(Sample(), 5), 315);
        return FindPath(Input, 5);
    }
}
