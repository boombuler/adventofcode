namespace AdventOfCode._2023;

using System.Numerics;
using Point = Point3D<double>;

class Day24 : Solution
{
    record Hailstone(Point Position, Point Velocity)
    {
        public Point2D<double> Intersect(Hailstone other)
        {
            var m1 = Velocity.Y / Velocity.X;
            var m2 = other.Velocity.Y / other.Velocity.X;
            if (m1 == m2)
                return Point2D<double>.Origin;

            var b1 = Position.Y - m1 * Position.X;
            var b2 = other.Position.Y - m2 * other.Position.X;

            var x = (b2 - b1) / (m1 - m2);
            if (((Velocity.X > 0) == (x > Position.X)) && 
                ((other.Velocity.X > 0) == (x > other.Position.X)))
                return (x, x * m1 + b1); // Only forward in time...
            return Point2D<double>.Origin;
        }
    }

    private static readonly Func<string, Hailstone[]> ParseHailstones = (
        from p in Parser.LongPoint3D + "@"
        from v in Parser.LongPoint3D
        select new Hailstone((p.X, p.Y, p.Z), (v.X, v.Y, v.Z))).List('\n');

    protected override long? Part1()
    {
        static long Solve(string input, double min, double max)
            => ParseHailstones(input).Pairs().Select(pair => pair.A.Intersect(pair.B))
                .Count(i => i.X >= min && i.X <= max && i.Y >= min && i.Y <= max);

        Assert(Solve(Sample(), 7, 27), 2);
        return Solve(Input, 200000000000000, 400000000000000);
    }

    private static long ThrowRock(string input)
    {
        var hailstones = (
            from hs in ParseHailstones(input)
            select (
                P: new BigInteger(hs.Position.X + hs.Position.Y + hs.Position.Z),
                V: new BigInteger(hs.Velocity.X + hs.Velocity.Y + hs.Velocity.Z)
            )).ToList();

        for (int velocity = 0; true; velocity++)
        {
            var (result, _) = hailstones
                .Select(i => (A: i.P, N: BigInteger.Abs(i.V - velocity)))
                .Unfold(i =>
                {
                    var ((a,n), rest) = i;
                    return (
                        (A: a, N: n),
                        rest.Where(x => MathExt.GCD(x.N, n) == 1).ToList()
                    );
                }, x => x.Any())
                .Where(f => f.N != 1)
                .Aggregate(MathExt.ChineseRemainder);

            if (hailstones.All(i => (result - i.P).Sign == (i.V - velocity).Sign))
                return (long)result;
        }
    }

    protected override long? Part2()
    {
        Assert(ThrowRock(Sample()), 47);
        return ThrowRock(Input);
    }
}
