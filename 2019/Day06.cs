namespace AdventOfCode._2019;

class Day06 : Solution
{
    private static long CountOrbits(string map)
    {
        var items = map.Lines().Select(l => l.Split(')')).ToLookup(p => p[0], p => p[1]);
        long result = 0;
        var todo = new Stack<(string Name, long Factor)>();
        todo.Push(("COM", 1));
        while (todo.TryPop(out var cur))
        {
            foreach (var item in items[cur.Name])
            {
                result += cur.Factor;
                todo.Push((item, cur.Factor + 1));
            }
        }
        return result;
    }

    private long? CountTransfers(string map)
    {
        var parents = map.Lines().Select(l => l.Split(')')).ToDictionary(p => p[1], p => p[0]);

        IEnumerable<string> GetOrbits(string src)
        {
            while (parents.TryGetValue(src, out var par))
            {
                yield return par;
                src = par;
            }
        }

        var santaOrbits = GetOrbits("SAN").ToList();

        long result = 0;
        foreach (var n in GetOrbits("YOU"))
        {
            result++;
            var idx = santaOrbits.IndexOf(n);
            if (idx > 0)
                return result + idx - 1;
        }
        Error("No common orbit");
        return null;
    }

    protected override long? Part1()
    {
        Assert(CountOrbits(Sample(nameof(Part1))), 42);
        return CountOrbits(Input);
    }

    protected override long? Part2()
    {
        Assert(CountTransfers(Sample(nameof(Part2))), 4);
        return CountTransfers(Input);
    }

}
