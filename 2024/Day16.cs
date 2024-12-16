namespace AdventOfCode._2024;

using Point = Point2D<int>;

class Day16 : Solution
{
    private IEnumerable<(long Score, IEnumerable<Point> Way)> GetWinningPaths(string input, Func<long, bool> continueWalking)
    {
        var map = input.AsMap();
        var start = map.First(n => n.Value == 'S').Index;

        var queue = new PriorityQueue<(ImmutableStack<Point>, Point), long>();
        queue.Enqueue((ImmutableStack<Point>.Empty.Push(start), Point.Right), 0);

        var maxScore = long.MaxValue;
        var maxTileScore = new Dictionary<(Point, Point), long>();
        while (queue.TryDequeue(out var state, out var score))
        {
            var (way, dir) = state;
            var pos = way.Peek();
            
            if (map[pos] == 'E')
            {
                maxScore = score;
                yield return (score, way);
                continue;
            }

            if (!continueWalking(score - maxTileScore.GetValueOrDefault((pos, dir), maxScore)))
                continue;
            maxTileScore[(pos, dir)] = score;
            
            void Enqueue(Point ndir, long extraScore)
            {
                if (map[pos + ndir] != '#')
                    queue.Enqueue((way.Push(pos + ndir), ndir), score + extraScore);
            }
            Enqueue(dir, 1);
            Enqueue(dir.RotateCW(), 1001);
            Enqueue(dir.RotateCCW(), 1001);
        }
    }

    protected override long? Part1()
    {
        long Solve(string input)
            => GetWinningPaths(input, scoreDelta => scoreDelta < 0).First().Score;
        Assert(Solve(Sample()), 7036);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        long Solve(string input)
            => GetWinningPaths(input, scoreDelta => scoreDelta <= 0)
                .SelectMany(p => p.Way).Distinct().Count();
        Assert(Solve(Sample()), 45);
        return Solve(Input);
    }
}
