namespace AdventOfCode._2024;

using Point = Point2D<int>;

class Day15 : Solution
{
    enum Tile { Free, Wall, Robot, BoxL, BoxR}

    private long RunRobot(string input, Func<string, string> expandMap)
    {
        var (mapStr, (dirStr, _)) = input.Split("\n\n");
        var map = expandMap(mapStr).AsMap(c => c switch
        {
            'O' => Tile.BoxL,
            'R' => Tile.BoxR,
            '#' => Tile.Wall,
            '@' => Tile.Robot,
            _ => Tile.Free
        });

        bool Move(Point src, Point dir)
        {
            if (WalkMap(src + dir, dir, Move))
            {
                map[src + dir] = map[src];
                map[src] = Tile.Free;
                return true;
            }
            return false;
        }

        bool CanMove(Point src, Point dir)
            => WalkMap(src, dir, (_, _) => true);

        bool WalkMap(Point src, Point dir, Func<Point, Point, bool> apply)
        {
            switch ((dir.Y == 0, map[src]))
            {
                case (_, Tile.Wall): return false;
                case (_, Tile.Free): return true;
                case (true, _):
                case (_, Tile.Robot):
                    return CanMove(src + dir, dir) && apply(src, dir);
                case (_, Tile.BoxR):
                    return WalkMap(src + Point.Left, dir, apply);
                default: // Left Box up or down
                    var boxR = src + Point.Right;
                    bool leftBoxOnly = map[boxR] != Tile.BoxR;

                    return CanMove(src + dir, dir) && (leftBoxOnly || CanMove(boxR + dir, dir)) &&
                        apply(src, dir) && (leftBoxOnly || apply(boxR, dir));
            }
        }

        _ = dirStr.Replace("\n", "").Select(Point.DirectionFromArrow).Aggregate(
            map.First(n => n.Value == Tile.Robot).Index,
            (pos, d) => Move(pos, d) ? pos + d : pos);

        return map.Where(n => n.Value == Tile.BoxL).Sum(n => n.Index.Y * 100 + n.Index.X);
    }

    protected override long? Part1()
    {
        long Solve(string input) => RunRobot(input, Functional.Identity);
        
        Assert(Solve(Sample()), 10092);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input) => RunRobot(input, m => m
            .Replace(".", "..")
            .Replace("#", "##")
            .Replace("O", "OR")
            .Replace("@", "@."));

        Assert(Solve(Sample()), 9021);
        return Solve(Input);
    }
}
