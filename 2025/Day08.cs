namespace AdventOfCode._2025;

using JunktionBox = Point3D<int>;

class Day08 : Solution
{
    private static IEnumerable<(JunktionBox A, JunktionBox B, List<List<JunktionBox>> Networks)> MergeNetworks(string input)
    {
        double Distance((JunktionBox a, JunktionBox b) p) => p.a.As<double>().StraightLineDistance(p.b.As<double>());

        var boxes = input.Lines().Select(Parser.IntPoint3D.MustParse).ToArray();

        var networkLookup = boxes.ToDictionary(b => b, b => new List<JunktionBox>() { b });
        var remainingNetworks = networkLookup.Values.ToList();
        foreach(var p in boxes.CombinationPairs().OrderBy(Distance))
        {
            var nA = networkLookup[p.A];
            var nB = networkLookup[p.B];

            if (nA != nB)
            {
                nA.AddRange(nB);
                foreach (var b in nB)
                    networkLookup[b] = nA;
                remainingNetworks.Remove(nB);
            }

            yield return (p.A, p.B, remainingNetworks);
        }
    }

    protected override long? Part1()
    {
        static long Solve(string input, int count)
            => MergeNetworks(input).Skip(count - 1).First().Networks.Select(n => n.Count).OrderDescending().Take(3).Aggregate((a, b) => a * b);

        Assert(Solve(Sample(), 10), 40);
        return Solve(Input, 1000); 
    }

    protected override long? Part2()
    {
        static long Solve(string input)
        {
            var (a, b, _) = MergeNetworks(input).TakeUntil(ev => ev.Networks.Count == 1).Last();
            return a.X * (long)b.X;
        }

        Assert(Solve(Sample()), 25272);
        return Solve(Input);
    }
}
