namespace AdventOfCode._2021;

using Point = Point2D<int>;

class Day15 : Solution
{
    class PathFinder(Dictionary<Point, long> riskMap) : AStar<Point>(Point.Origin)
    {
        protected override long Distance(Point one, Point another)
        {
            var distance = one.ManhattanDistance(another);
            if (distance == 1)
                return riskMap[another];
            return distance;
        }

        protected override IEnumerable<Point> NeighboursOf(Point node)
            => node.Neighbours().Where(riskMap.ContainsKey);
    }

    private static long? FindPath(string input, int expandMap = 1)
    {
        var baseMap = input.AsMap(c => c - '0');
        var (width, height) = (baseMap.Width, baseMap.Height);
        var riskMap = new Dictionary<Point, long>();

        foreach (var of in Point.Range(Point.Origin, (expandMap - 1, expandMap - 1)))
        {
            var offset = new Point(of.X * width, of.Y * height);
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
