namespace AdventOfCode._2024;

using Point = Point2D<int>;

class Day06 : Solution
{
    IEnumerable<(Point Position, Point Direction)> WalkMap(StringMap<char> map, Point? start = null)
        => (Pos: (start ?? map.Find('^')) - Point.Up, Dir: Point.Up)
            .Unfold(s => 
            {
                var n = s.Pos + s.Dir;
                if (map.GetValueOrDefault(n) == '#')
                    return (s.Pos, (-s.Dir.Y, s.Dir.X));
                return (n, s.Dir);
            }).TakeWhile(x => map.Contains(x.Pos));

    protected override long? Part1() 
    {
        long Solve(string input)
            => WalkMap(input.AsMap()).DistinctBy(x => x.Position).Count();

        Assert(Solve(Sample()), 41);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        bool IsLoop(StringMap<char> map, Point start)
        {
            var visited = new HashSet<(Point, Point)>();
            return WalkMap(map, start).Pairwise().Where(pair => pair.A.Direction != pair.B.Direction).Any(pair => !visited.Add(pair.B));
        }

        int Solve(string input)
        {
            var map = input.AsMap();
            var start = map.Find('^');
            return (
                from replace in WalkMap(map).Select(x => x.Position).Distinct()
                where replace != start && IsLoop(new StringMap<char>(map) { [replace] = '#' }, start)
                select 1
            ).Count();
        }

        Assert(Solve(Sample()), 6);
        return Solve(Input);
    }
}
