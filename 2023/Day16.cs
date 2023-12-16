namespace AdventOfCode._2023;

using Point = Point2D<int>;
using static Point2D<int>;

class Day16 : Solution
{
    private static int CountTiles(StringMap<char> map, Point startPos, Point startDir)
    {
        static Point[] NextDirections(char c, Point d)
            => c switch
            {
                '\\' => [(d.Y, d.X)],
                '/' => [(-d.Y, -d.X)],
                '|' when d == Left || d == Right => [Up, Down],
                '-' when d == Up || d == Down => [Left, Right],
                _ => [d]
            };

        var seen = new HashSet<(Point Pos, Point)>();
        var open = new Stack<(Point Pos, Point Dir)>([(startPos - startDir, startDir)]); 

        while (open.TryPop(out var cur))
        {
            if (!seen.Add(cur))
                continue;

            var newPos = cur.Pos + cur.Dir;
            if (map.Contains(newPos))
                NextDirections(map[newPos], cur.Dir).ForEach(d => open.Push((newPos, d)));
        }
        return seen.Select(n => n.Pos).Distinct().Count() - 1;
    }

    protected override long? Part1()
    {
        static long Solve(string input) =>
            CountTiles(input.AsMap(), (0, 0), Point.Right);
        
        Assert(Solve(Sample()), 46);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static int Solve(string input)
        {
            var map = input.AsMap();

            return (
                from pt in map
                from d in new[] { Up, Down, Left, Right }
                where !map.Contains(pt.Index - d)
                select CountTiles(map, pt.Index, d)
            ).Max();
        }

        Assert(Solve(Sample()), 51);
        return Solve(Input);
    }
}
