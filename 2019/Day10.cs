using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

namespace AdventOfCode._2019
{
    class Day10 : Solution
    {
        private Point2D Normalize(Point2D center, Point2D pt)
        {
            pt = pt - center;
            var gcd = Math.Abs(MathExt.GCD(pt.X, pt.Y));
            return (pt.X / gcd, pt.Y / gcd);
        }

        private double Degrees(Point2D direction)
        {
            double Mod(double a, double b) => a - Math.Floor(a / b) * b;
            var radian = Math.Atan2(direction.Y, direction.X);
            return Mod(radian * 180 / Math.PI + 90, 360);
        }

        private long GetVisibleAsteroids(Point2D src, IEnumerable<Point2D> other)
            => other.Where(o => o != src).Select(o => Normalize(src, o)).Distinct().Count();

        private long MostVisibleAsteroids(string input)
        {
            var asteroids = input.Lines().SelectMany((l, y) => l.Select((c, x) => c == '#' ? new Point2D(x, y) : null)).Where(a => a != null).ToList();

            return asteroids.Select(a => GetVisibleAsteroids(a, asteroids)).Max();
        }

        private IEnumerable<Point2D> ShootingOrder(string input)
        {
            var asteroids = input.Lines().SelectMany((l, y) => l.Select((c, x) => c == '#' ? new Point2D(x, y) : null)).Where(a => a != null).ToList();
            var (_, center) = asteroids.MinMaxBy(a => GetVisibleAsteroids(a, asteroids));
            var targets = asteroids
                .Where(a => a != center)
                .Select(a => new { Point = a, Direction = Normalize(center, a), Distance = center.ManhattanDistance(a) })
                .GroupBy(a => a.Direction)
                .OrderBy(g => Degrees(g.Key))
                .Select(g => g.OrderBy(e => e.Distance).Select(i => i.Point).ToList())
                .ToList();
            var dir = 0;
            var itm = 0;
            var max = targets.Max(l => l.Count);
            while(itm < max)
            {
                var direction = targets[dir];
                if (direction.Count > itm)
                    yield return direction[itm];
                if (++dir == targets.Count)
                {
                    dir = 0;
                    itm++;
                }
            }
        }
        private long Target200(string input)
        {
            var item = ShootingOrder(input).Skip(199).First();
            return item.X * 100 + item.Y;
        }


        protected override long? Part1()
        {
            Assert(MostVisibleAsteroids(Sample("0")), 8);
            Assert(MostVisibleAsteroids(Sample("1")), 33);
            Assert(MostVisibleAsteroids(Sample("2")), 35);
            Assert(MostVisibleAsteroids(Sample("3")), 41);
            Assert(MostVisibleAsteroids(Sample("4")), 210);
            return MostVisibleAsteroids(Input);
        }

        protected override long? Part2()
        {
            Assert(Target200(Sample("4")), 802);
            return Target200(Input);
        }
    }
}
