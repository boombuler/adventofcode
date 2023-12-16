namespace AdventOfCode._2023;

using Point = Point2D<int>;

class Day16 : Solution
{
    private static int CountTiles(StringMap<char> map, Point startPos, Point startDir)
    {
        var seen = new HashSet<(Point Pos, Point)>();
        var open = new Stack<(Point Pos, Point Dir)>([(startPos - startDir, startDir)]); 

        while (open.TryPop(out var cur))
        {
            if (!seen.Add(cur))
                continue;

            var newPos = cur.Pos + cur.Dir;
            if (!map.Contains(newPos))
                continue;

            switch (map[newPos])
            {
                case '\\':
                    open.Push((newPos, (cur.Dir.Y, cur.Dir.X))); break;
                case '/':
                    open.Push((newPos, (-cur.Dir.Y, -cur.Dir.X))); break;
                case '|' when cur.Dir == Point.Left || cur.Dir == Point.Right:
                    open.Push((newPos, Point.Up));
                    open.Push((newPos, Point.Down));
                    break;
                case '-' when cur.Dir == Point.Up || cur.Dir == Point.Down:
                    open.Push((newPos, Point.Left));
                    open.Push((newPos, Point.Right));
                    break;
                default:
                    open.Push((newPos, cur.Dir));
                    break;
            }
        }
        return seen.Select(n => n.Pos).Distinct().Count() - 1;
    }

    private static long Solve2(string input)
    {
        var map = input.AsMap();
        
        long max = 0;
        for (int x = 0; x < map.Width; x++)
        {
            max = Math.Max(max, CountTiles(map, (x, 0), Point.Down));
            max = Math.Max(max, CountTiles(map, (x, map.Height-1), Point.Up));
        }

        for (int y = 0; y < map.Height; y++)
        {
            max = Math.Max(max, CountTiles(map, (0, y), Point.Right));
            max = Math.Max(max, CountTiles(map, (map.Width-1, y), Point.Left));
        }
        return max;
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
        Assert(Solve2(Sample()), 51);
        return Solve2(Input);
    }
}
