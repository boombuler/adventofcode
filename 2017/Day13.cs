namespace AdventOfCode._2017;

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day13 : Solution
{
    private static IEnumerable<(Func<int, bool> passes, int severity)> ReadLayers(string layers)
    {
        foreach (var line in layers.Lines())
        {
            var parts = line.Split(':').Select(p => int.Parse(p.Trim())).ToArray();
            int layer = parts[0];
            int depth = parts[1];
            int mod = (depth - 1) * 2;
            var passes = new Func<int, bool>(offset => ((-offset - layer) % mod) != 0);

            yield return (passes, layer * depth);
        }
    }

    private static long GetSeverity(string layerFile)
        => ReadLayers(layerFile).Where(l => !l.passes(0)).Sum(l => l.severity);

    private static long GetDelayTime(string layerFile)
        => ReadLayers(layerFile).Select(l => l.passes)
            .Aggregate(EnumerableHelper.Generate(), (sequence, filter) => sequence.Where(filter))
            .First();

    protected override long? Part1()
    {
        Assert(GetSeverity(Sample()), 24);
        return GetSeverity(Input);
    }

    protected override long? Part2()
    {
        Assert(GetDelayTime(Sample()), 10);
        return GetDelayTime(Input);
    }
}
