﻿using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2021
{

    class Day19 : Solution
    {
        class Cluster
        {
            public ImmutableArray<ImmutableArray<Point3D>> Orientations { get; }

            public Cluster(IEnumerable<Point3D> beacons)
            {
                Orientations = GetOrientations().Select(beacons.Select).Select(ImmutableArray.ToImmutableArray).ToImmutableArray();
            }

            private static IEnumerable<Func<Point3D, Point3D>> GetOrientations()
            {
                IEnumerable<Mat4> Rotate(Point3D direction)
                    => Mat4.Identity.Unfold(m => m.Rotate90Degree(direction)).Take(4);

                return Rotate((0, 1, 0)).Concat(Rotate((0, 0, 1)))
                    .SelectMany(r => Rotate((1, 0, 0)).Select(r.Combine))
                    .Distinct()
                    .Select(m => new Func<Point3D, Point3D>(m.Apply));
            }
        }

        record Scanner(ImmutableArray<Point3D> Beacons, Point3D Offset);

        private List<Cluster> Parse(string input)
        {
            var result = new List<Cluster>();
            var curList = new List<Point3D>();
            var parsePt = new Regex(@"(?<X>-?\d+),(?<Y>-?\d+),(?<Z>-?\d+)");
            foreach(var line in input.Lines())
            {
                if (parsePt.TryMatch<Point3D>(line, out var pt))
                    curList.Add(pt);
                else if (curList.Any())
                {
                    result.Add(new Cluster(curList));
                    curList.Clear();
                }
            }
            result.Add(new Cluster(curList));
            return result;
        }

        private Scanner Align(Cluster cluster, Scanner scanner)
        {
            const int MIN_OVERLAP = 12;
            foreach(var oriented in cluster.Orientations)
            {
                var offset = scanner.Beacons.SelectMany(b => oriented.Select(o => b - o))
                    .GroupBy(off => off).Where(grp => grp.Count() >= MIN_OVERLAP)
                    .Select(grp => grp.Key).FirstOrDefault();

                if (offset != null)
                    return new Scanner(oriented.Select(p => p + offset).ToImmutableArray(), offset);
            }
            return null;
        }

        private IEnumerable<Scanner> AlignAllScanners(string scanResult)
        {
            var unmappedClusters = Parse(scanResult).ToHashSet();
            var originCluster = unmappedClusters.First();
            var originScanner = new Scanner(originCluster.Orientations.First(), Point3D.Origin);
            var scanners = new List<Scanner>() { originScanner };
            unmappedClusters.Remove(originCluster);

            var open = new Queue<(Cluster, Scanner)>(unmappedClusters.Select(c => (c, originScanner)));
            while (open.TryDequeue(out var current))
            {
                var (cluster, scanner) = current;
                if (!unmappedClusters.Contains(cluster))
                    continue;

                var result = Align(cluster, scanner);
                if (result == null)
                    continue;
                
                scanners.Add(result);
                unmappedClusters.Remove(cluster);
                foreach (var c in unmappedClusters)
                    open.Enqueue((c, result));
            }
            return scanners;
        }

        protected override long? Part1()
        {
            int DistinctBeacons(string scan) 
                => AlignAllScanners(scan).SelectMany(s => s.Beacons).Distinct().Count();
            Assert(DistinctBeacons(Sample()), 79);
            return DistinctBeacons(Input);
        }

        protected override long? Part2()
        {
            long MaxDistance(string scan)
                => AlignAllScanners(scan).Combinations(2).Max(w => w.First().Offset.ManhattanDistance(w.Last().Offset));
            Assert(MaxDistance(Sample()), 3621);
            return MaxDistance(Input);
        }
    }
}
