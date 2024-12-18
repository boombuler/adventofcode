namespace AdventOfCode._2024;

using Point = Point2D<int>;

class Day18 : Solution<long, Point>
{
    class PathFinder(Rect2D<int> bounds, HashSet<Point> walls) 
        : AStar<Point>(Point.Origin)
    {
        protected override long Distance(Point from, Point to)
            => from.ManhattanDistance(to);
        protected override IEnumerable<Point> NeighboursOf(Point node)
            => node.Neighbours().Where(n => bounds.Contains(n) && !walls.Contains(n));
    }

    protected override long Part1()
    {
        long Solve(string input, Point size, int count)
        {
            var walls = Parser.IntPoint2D.List('\n').MustParse(input).Take(count).ToHashSet();
            var pf = new PathFinder(new Rect2D<int>(Point.Origin, size) , walls);
            return pf.ShortestPath(size).Cost;
        }

        Assert(Solve(Sample(), (6,6), 12), 22);
        return Solve(Input, (70,70), 1024);
    }

    protected override Point Part2()
    {
        Point Solve(string input, Point size)
        {
            var bounds = new Rect2D<int>(Point.Origin, size);
            var walls = Parser.IntPoint2D.List('\n').MustParse(input).ToList();

            bool HasValidPath(int idx)
                => new PathFinder(bounds, walls.Take(idx).ToHashSet()).ShortestPath(size).Cost >= 0;

            return walls[MathExt.Bisect(0, walls.Count-1, HasValidPath)];
        }

        Assert(Solve(Sample(), (6, 6)), new Point(6,1));
        return Solve(Input, (70, 70));
    }
}
