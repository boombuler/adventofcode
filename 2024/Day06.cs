namespace AdventOfCode._2024;

using Pose = (Point2D<int> Position, Point2D<int> Direction);

class Day06 : Solution
{
    static Pose Prev(Pose p) => (p.Position - p.Direction, p.Direction);

    static IEnumerable<Pose> WalkMap(StringMap<char> map, Pose? start = null)
        => Prev(start ?? (map.Find('^') ?? throw new InvalidInputException(), Point2D<int>.Up))
            .Unfold(s => 
            {
                var n = s.Position + s.Direction;
                if (map.GetValueOrDefault(n) == '#')
                    return (s.Position, (-s.Direction.Y, s.Direction.X));
                return (n, s.Direction);
            }).TakeWhile(x => map.Contains(x.Position));

    protected override long? Part1() 
    {
        static long Solve(string input)
            => WalkMap(input.AsMap()).DistinctBy(x => x.Position).Count();

        Assert(Solve(Sample()), 41);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static bool IsLoop(StringMap<char> map, Pose pose)
        {
            var visited = new HashSet<Pose>();
            return WalkMap(map, pose)
                .Pairwise().Where(pair => pair.A.Direction != pair.B.Direction) // Only remember turns
                .Any(pair => !visited.Add(pair.B));
        }

        static int Solve(string input)
        {
            var map = input.AsMap();
            var start = map.Find('^');

            return (
                from x in WalkMap(map).DistinctBy(x => x.Position).AsParallel().AsUnordered()
                let newMap = new StringMap<char>(map) { [x.Position] = '#' }
                where x.Position != start && IsLoop(newMap, Prev(x))
                select 1
            ).Count();
        }

        Assert(Solve(Sample()), 6);
        return Solve(Input);
    }
}
