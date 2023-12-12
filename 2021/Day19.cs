namespace AdventOfCode._2021;

using Mat = Mat4<int>;
using Point = Point3D<int>;

class Day19 : Solution
{
    class Cluster(IEnumerable<Point> beacons)
    {
        public ImmutableArray<ImmutableArray<Point>> Orientations { get; }
            = GetOrientations().Select(beacons.Select).Select(ImmutableArray.ToImmutableArray).ToImmutableArray();

        private static IEnumerable<Func<Point, Point>> GetOrientations()
        {
            IEnumerable<Mat> Rotate(Point direction)
                => Mat.Identity.Unfold(m => m.Rotate90Degree(direction)).Take(4);

            return Rotate((0, 1, 0)).Concat(Rotate((0, 0, 1)))
                .SelectMany(r => Rotate((1, 0, 0)).Select(r.Combine))
                .Distinct()
                .Select(m => new Func<Point, Point>(m.Apply));
        }
    }

    record Scanner(ImmutableArray<Point> Beacons, Point Offset);

    private static List<Cluster> Parse(string input)
    {
        var result = new List<Cluster>();
        var curList = new List<Point>();
        var parsePt = new Regex(@"(?<X>-?\d+),(?<Y>-?\d+),(?<Z>-?\d+)");
        foreach (var line in input.Lines())
        {
            if (parsePt.TryMatch<Point>(line, out var pt))
                curList.Add(pt);
            else if (curList.Count > 0)
            {
                result.Add(new Cluster(curList));
                curList.Clear();
            }
        }
        result.Add(new Cluster(curList));
        return result;
    }

    private static Scanner Align(Cluster cluster, Scanner scanner)
    {
        const int MIN_OVERLAP = 12;
        foreach (var oriented in cluster.Orientations)
        {
            var offset = scanner.Beacons.SelectMany(b => oriented.Select(o => b - o))
                .GroupBy(off => off).Where(grp => grp.Count() >= MIN_OVERLAP)
                .Select(grp => grp.Key).FirstOrDefault();

            if (offset != null)
                return new Scanner(oriented.Select(p => p + offset).ToImmutableArray(), offset);
        }
        return null;
    }

    private static IEnumerable<Scanner> AlignAllScanners(string scanResult)
    {
        var unmappedClusters = Parse(scanResult).ToHashSet();
        var originCluster = unmappedClusters.First();
        var originScanner = new Scanner(originCluster.Orientations.First(), Point.Origin);
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
        static int DistinctBeacons(string scan)
            => AlignAllScanners(scan).SelectMany(s => s.Beacons).Distinct().Count();
        Assert(DistinctBeacons(Sample()), 79);
        return DistinctBeacons(Input);
    }

    protected override long? Part2()
    {
        static long MaxDistance(string scan)
            => AlignAllScanners(scan).Pairs().Max(w => w.A.Offset.ManhattanDistance(w.B.Offset));
        Assert(MaxDistance(Sample()), 3621);
        return MaxDistance(Input);
    }
}
