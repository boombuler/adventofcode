using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode._2019
{
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
            private Moon(long x, long y, long z)
            {
                Velocity = Point3D.Origin;
                Position = (x, y, z);
            }

            private long AbsSum(Point3D pt)
                => Math.Abs(pt.X) + Math.Abs(pt.Y) + Math.Abs(pt.Z);

            public void ApplyGravity(Moon other)
            {
                Velocity += (
                    Math.Sign(other.Position.X.CompareTo(Position.X)),
                    other.Position.Y.CompareTo(Position.Y),
                    other.Position.Z.CompareTo(Position.Z)
                );
            }

            public void Step()
            {
                Position += Velocity;
            }
        }
        
        
        private void StepSystem(List<Moon> moons)
        {
            foreach(var grp in moons.Combinations(2))
            {
                var (m1, (m2, _)) = grp;
                m1.ApplyGravity(m2);
                m2.ApplyGravity(m1);
            }
            foreach (var m in moons)
                m.Step();
        }

        private long SimulateSystem(string input, int steps)
        {
            var moons = input.Lines().Select(Moon.Parse).ToList();
            for (int i = 0; i < steps; i++)
                StepSystem(moons);
            return moons.Sum(m => m.TotalEnergy);
        }

        private long LCM(long a, long b)
        {
            long mult = a * b;
            while (a != b)
            {
                if (a > b)
                    a = a - b;
                else
                    b = b - a;
            }
            return mult / a;
        }

        private long GetLoopCount(string input)
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
            while(directions.Count > 0)
            {
                StepSystem(moons);
                loop++;

                if (directions.RemoveAll(d => d()) > 0)
                    result = LCM(result, loop);
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
}
