using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode._2018
{
    class Day23 : Solution
    {
        class BoundingBox
        {
            private readonly Point3D fSize;
            public Point3D Min { get; }
            public Point3D Max { get; }
            public long DistanceToOrigin { get; }
            public bool IsPoint => fSize.X == 1 && fSize.Y == 1 && fSize.Z == 1;
            
            public BoundingBox(Point3D min, Point3D size)
            {
                this.Min = min;
                this.Max = min + size - (1,1,1);
                this.fSize = size;
                this.DistanceToOrigin = Math.Min(
                    Max.ManhattanDistance(Point3D.Origin),
                    Min.ManhattanDistance(Point3D.Origin)
                );
            }

            public IEnumerable<BoundingBox> Divide()
            {
                BoundingBox MkBox(int i)
                {
                    bool IsSet(int bit) => ((i >> bit) & 1) != 0;
                    var pos  = fSize.MapComponents((v, b) => IsSet(b) ? v / 2 : 0);
                    var size = fSize.MapComponents((v, b) => v / (IsSet(b) ? 1 : 2));
                    return new BoundingBox(Min + pos, size - pos);
                }
                return Enumerable.Range(0, 8).Select(MkBox).Where(box => box.fSize.X > 0 && box.fSize.Y > 0 && box.fSize.Z > 0);
            }
        }

        class NanoBot
        {
            public static readonly Func<string, NanoBot> Parse = new Regex(@"pos=\<(?<x>-?\d+),(?<y>-?\d+),(?<z>-?\d+)\>, r=(?<r>\d+)").ToFactory<NanoBot>();
            public Point3D Location { get; }
            public long Radius { get; }
            public NanoBot(long x, long y, long z, long r)
                => (Location, Radius) = ((x, y, z), r);

            public bool Reaches(Point3D pt) => Location.ManhattanDistance(pt) <= Radius;
            public bool Intersects(BoundingBox box)
            {
                var dx = Math.Max(0, Math.Max(box.Min.X - Location.X, Location.X - box.Max.X));
                var dy = Math.Max(0, Math.Max(box.Min.Y - Location.Y, Location.Y - box.Max.Y));
                var dz = Math.Max(0, Math.Max(box.Min.Z - Location.Z, Location.Z - box.Max.Z));

                return dx + dy + dz <= Radius;
            }
        }

        public int CountInRangeOfStrongestSignal(string nanoBots)
        {
            var bots = nanoBots.Lines().Select(NanoBot.Parse).ToList();
            var (_, maxBot) = bots.MinMaxBy(b => b.Radius);
            return bots.Select(b => b.Location).Count(maxBot.Reaches);
        }

        public long DistanceToBestSpot(string nanoBots)
        {
            var bots = nanoBots.Lines().Select(NanoBot.Parse).ToArray();

            var (minX, maxX) = bots.MinMax(b => b.Location.X);
            var (minY, maxY) = bots.MinMax(b => b.Location.Y);
            var (minZ, maxZ) = bots.MinMax(b => b.Location.Z);
            var min = new Point3D(minX, minY, minZ);
            var max = new Point3D(maxX, maxY, maxZ);

            var q = new MaxHeap<(BoundingBox BBox, NanoBot[] Bots)>(
                ComparerBuilder<(BoundingBox BBox, NanoBot[] Bots)>
                    .CompareBy(x => x.Bots.Length)
                    .ThenByDesc(x => x.BBox.DistanceToOrigin)
            );

            q.Push((new BoundingBox(min, max - min + (1,1,1)), bots));

            while (q.TryPop(out var cur))
            {
                if (cur.BBox.IsPoint)
                    return cur.BBox.DistanceToOrigin;
                
                foreach (var box in cur.BBox.Divide())
                    q.Push((box, cur.Bots.Where(b => b.Intersects(box)).ToArray()));    
            }

            return -1;
        }

        protected override long? Part1()
        {
            Assert(CountInRangeOfStrongestSignal(Sample(nameof(Part1))), 7);
            return CountInRangeOfStrongestSignal(Input);
        }

        protected override long? Part2()
        {
            Assert(DistanceToBestSpot(Sample(nameof(Part2))), 36);
            return DistanceToBestSpot(Input);
        }
    }
}
