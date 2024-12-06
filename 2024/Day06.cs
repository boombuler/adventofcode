namespace AdventOfCode._2024;

using Point = Point2D<int>;

class Day06 : Solution
{
    IEnumerable<(Point Position, Point Direction)> WalkMap(StringMap<char> map)
    {
        Point[] directions = [Point.Up, Point.Right, Point.Down, Point.Left];
        var start = (Position: map.First(n => n.Value == '^').Index, Direction: 0);
        return start
            .Unfold(s =>
            {
                var n = s.Position + directions[s.Direction];
                if (map.GetValueOrDefault(n) == '#')
                    return (s.Position, (s.Direction + 1) % directions.Length);
                return (n, s.Direction);
            })
            .Prepend(start).TakeWhile(x => map.Contains(x.Position))
            .Select(x => (x.Position, directions[x.Direction]));
    }

    protected override long? Part1() 
    {
        long Solve(string input)
            => WalkMap(input.AsMap()).DistinctBy(x => x.Position).Count();

        Assert(Solve(Sample()), 41);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        bool IsLoop(StringMap<char> map)
        {
            var visited = new HashSet<(Point, Point)>();
            return WalkMap(map).Any(x => !visited.Add(x));
        }

        int Solve(string input)
        {
            var map = input.AsMap();
            
            return (
                // This could be optimized by checking if placing a wall
                // at the given position could even create a loop. To create a look
                // there needs to be a wall in the newly created path.
                from replace in WalkMap(map).Select(x => x.Position + x.Direction).Distinct()
                where map.TryGetValue(replace, out var cur) && cur == '.'
                where IsLoop(new StringMap<char>(map) { [replace] = '#' })
                select 1
            ).Count();
        }

        Assert(Solve(Sample()), 6);
        return Solve(Input);
    }
}
