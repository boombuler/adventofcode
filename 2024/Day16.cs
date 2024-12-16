namespace AdventOfCode._2024;

using Pose = (Point2D<int> Loc, Point2D<int> Dir);

class Day16 : Solution
{
    private IEnumerable<(long Score, IEnumerable<Pose> Way)> GetWinningPaths(string input, Func<long, bool> continueWalking)
    {
        var map = input.AsMap();
        var start = map.First(n => n.Value == 'S').Index;

        var queue = new PriorityQueue<ImmutableStack<Pose>, long>();
        queue.Enqueue(ImmutableStack<Pose>.Empty.Push((start, (1, 0))), 0);

        var maxScore = long.MaxValue;
        var maxTileScore = new Dictionary<Pose, long>();
        while (queue.TryDequeue(out var way, out var score))
        {
            var p = way.Peek();
            if (!continueWalking(score - maxTileScore.GetValueOrDefault(p, maxScore)))
                continue;
            maxTileScore[p] = score;

            void Enqueue(Pose p, long extraCost)
                => queue.Enqueue(way.Push(p), score + extraCost);

            switch (map[p.Loc + p.Dir])
            {
                case 'E':
                    maxScore = score+1;
                    yield return (score+1, way.Push((p.Loc + p.Dir, p.Dir)));
                    break;
                case '.':
                    Enqueue((p.Loc + p.Dir, p.Dir), 1);
                    break;
            }
            Enqueue((p.Loc, p.Dir.RotateCW()), 1000);
            Enqueue((p.Loc, p.Dir.RotateCCW()), 1000);
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
                .SelectMany(p => p.Way.Select(wp => wp.Loc)).Distinct().Count();
        Assert(Solve(Sample()), 45);
        return Solve(Input);
    }
}
