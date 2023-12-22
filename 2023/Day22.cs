namespace AdventOfCode._2023;

using static Parser;
using Point = Point3D<int>;

class Day22 : Solution
{
    record Brick(Point Min, Point Max)
    {        
        public bool Intersect(Brick other)
            => (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
               (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
               (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);

        public Brick Down() => new ((Min.X, Min.Y, Min.Z-1), (Max.X, Max.Y, Max.Z-1));
    }

    private static readonly Func<string, Brick[]> BrickParser = (
        from s in IntPoint3D + "~"
        from e in IntPoint3D
        let z = s.Z < e.Z ? (s.Z, e.Z) : (e.Z, s.Z)
        let y = s.Y < e.Y ? (s.Y, e.Y) : (e.Y, s.Y)
        let x = s.X < e.X ? (s.X, e.X) : (e.X, s.X)
        select new Brick((x.Item1, y.Item1, z.Item1), (x.Item2, y.Item2, z.Item2))
    ).List('\n');

    private static Dictionary<Brick, Brick[]> GetBrickSupports(string input)
    {
        var unsettled = BrickParser(input).OrderBy(b => b.Min.Z).ToList();
        var settledZMax = new Dictionary<int, List<Brick>>();

        while (unsettled.Count > 0)
        {
            var nextPos = new List<Brick>();
            foreach (var b in unsettled)
            {
                var moved = b.Down();
                if (moved.Min.Z == 0 || (settledZMax.GetValueOrDefault(moved.Min.Z)?.Where(moved.Intersect).Any() ?? false))
                    settledZMax.GetOrAdd(b.Max.Z, () => new List<Brick>()).Add(b);
                else
                    nextPos.Add(moved);
            }
            unsettled = nextPos;
        }
        var settled = settledZMax.SelectMany(n => n.Value).ToList();
        return settled.ToDictionary(s => s,
            s => settled.Except([s]).Where(s.Down().Intersect).ToArray());
    }

    private static long Solve(string input)
    {
        var supports = GetBrickSupports(input);
        return supports.Keys.Except(supports.Where(g => g.Value.Length == 1).SelectMany(g => g.Value)).Count();
    }

    private static long Solve2(string input)
    {
        var supports = GetBrickSupports(input);
        
        long ChainReaction(Brick b)
        {
            HashSet<Brick> falling = [b];
            bool anyAdded;
            do
            {
                anyAdded = false;
                foreach(var (bs, sup) in supports)
                {
                    if (falling.Contains(bs) || sup.Length == 0 || !sup.All(falling.Contains))
                        continue;
                    falling.Add(bs);
                    anyAdded = true;
                }
            } while (anyAdded);
            return falling.Count - 1;
        }

        return supports.Keys.Sum(ChainReaction);
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample()), 5);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        Assert(Solve2(Sample()), 7);
        return Solve2(Input);
    }
}
