namespace AdventOfCode._2020;

class Day17 : Solution
{
    private static long Emulate<T>(HashSet<T> activeCells, int rounds, GameOfLife.GetNeighbours<T> getNeighbours)
        => GameOfLife.Emulate<T>(activeCells, rounds, getNeighbours, (wasActive, active) => active == 3 || (wasActive && active == 2));

    private static IEnumerable<(int x, int y)> ReadActive(string initial)
    {
        int y = 0;
        foreach (var line in initial.Lines())
        {
            for (int x = 0; x < line.Length; x++)
                if (line[x] == '#')
                    yield return (x, y);
            y++;
        }
    }

    private static long GoL3D(string initial, int rounds)
    {
        var activeCells = new HashSet<(int x, int y, int z)>(ReadActive(initial).Select(c => (c.x, c.y, 0)));

        static IEnumerable<(int x, int y, int z)> Neighbours((int x, int y, int z) pt)
        {
            return from x in Enumerable.Range(pt.x - 1, 3)
                   from y in Enumerable.Range(pt.y - 1, 3)
                   from z in Enumerable.Range(pt.z - 1, 3)
                   where (x != pt.x || y != pt.y || z != pt.z)
                   select (x, y, z);
        }

        return Emulate(activeCells, rounds, Neighbours);
    }

    private static long GoL4D(string initial, int rounds)
    {
        var activeCells = new HashSet<(int x, int y, int z, int w)>(ReadActive(initial).Select(c => (c.x, c.y, 0, 0)));

        static IEnumerable<(int x, int y, int z, int w)> Neighbours((int x, int y, int z, int w) pt)
        {
            return from x in Enumerable.Range(pt.x - 1, 3)
                   from y in Enumerable.Range(pt.y - 1, 3)
                   from z in Enumerable.Range(pt.z - 1, 3)
                   from w in Enumerable.Range(pt.w - 1, 3)
                   where (x != pt.x || y != pt.y || z != pt.z || w != pt.w)
                   select (x, y, z, w);
        }

        return Emulate(activeCells, rounds, Neighbours);
    }

    protected override long? Part1()
    {
        Assert(GoL3D(Sample(), 6), 112);
        return GoL3D(Input, 6);
    }

    protected override long? Part2()
    {
        Assert(GoL4D(Sample(), 6), 848);
        return GoL4D(Input, 6);
    }
}
