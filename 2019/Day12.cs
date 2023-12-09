namespace AdventOfCode._2019;

using System.Diagnostics.CodeAnalysis;

class Day12 : Solution
{
    class Moon
    {
        public static readonly Func<string, Moon> Parse = new Regex(@"\<x=(?<X>-?\d+), y=(?<Y>-?\d+), z=(?<Z>-?\d+)\>", RegexOptions.Compiled).ToFactory<Moon>();
        public Point3D Position { get; private set; }
        public Point3D Velocity { get; private set; }
        public long PotentialEnergy => AbsSum(Position);
        public long KineticEnergy => AbsSum(Velocity);
        public long TotalEnergy => PotentialEnergy * KineticEnergy;

        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by factory (reflection)")]
        private Moon(long x, long y, long z)
        {
            Velocity = Point3D.Origin;
            Position = (x, y, z);
        }

        private static long AbsSum(Point3D pt)
            => Math.Abs(pt.X) + Math.Abs(pt.Y) + Math.Abs(pt.Z);

        public void ApplyGravity(Moon other)
        {
            Velocity += (
                Math.Sign(other.Position.X.CompareTo(Position.X)),
                other.Position.Y.CompareTo(Position.Y),
                other.Position.Z.CompareTo(Position.Z)
            );
        }

        public void Step() => Position += Velocity;
    }

    private static void StepSystem(List<Moon> moons)
    {
        foreach (var (m1, m2) in moons.Pairs())
        {
            m1.ApplyGravity(m2);
            m2.ApplyGravity(m1);
        }
        foreach (var m in moons)
            m.Step();
    }

    private static long SimulateSystem(string input, int steps)
    {
        var moons = input.Lines().Select(Moon.Parse).ToList();
        for (int i = 0; i < steps; i++)
            StepSystem(moons);
        return moons.Sum(m => m.TotalEnergy);
    }

    private static long GetLoopCount(string input)
    {
        var moons = input.Lines().Select(Moon.Parse).ToList();
        var directions = new List<Func<bool>>()
            {
                () => moons.All(m => m.Velocity.X == 0),
                () => moons.All(m => m.Velocity.Y == 0),
                () => moons.All(m => m.Velocity.Z == 0),
            };

        long result = 1;
        long loop = 0;
        while (directions.Count > 0)
        {
            StepSystem(moons);
            loop++;

            if (directions.RemoveAll(d => d()) > 0)
                result = MathExt.LCM(result, loop);
        }
        return result * 2;
    }

    protected override long? Part1()
    {
        Assert(SimulateSystem(Sample(), 10), 179);
        return SimulateSystem(Input, 1000);
    }

    protected override long? Part2()
    {
        Assert(GetLoopCount(Sample()), 2772);
        return GetLoopCount(Input);
    }
}
