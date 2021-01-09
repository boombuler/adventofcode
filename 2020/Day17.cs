using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventOfCode._2020
{
    class Day17 : Solution
    {
        private long Emulate<T>(HashSet<T> activeCells, int rounds, GameOfLife.GetNeighbours<T> getNeighbours)
            => GameOfLife.Emulate<T>(activeCells, rounds, getNeighbours, (wasActive, active) => active == 3 || (wasActive && active == 2));
        
        private IEnumerable<(int x, int y)> ReadActive(string initial)
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
        
        
        private long GoL3D(string initial, int rounds)
        {
            var activeCells = new HashSet<(int x, int y, int z)>(ReadActive(initial).Select(c => (c.x, c.y, 0)));

            IEnumerable<(int x, int y, int z)> Neighbours((int x, int y, int z) pt)
            {
                for (int x = pt.x - 1; x <= pt.x + 1; x++)
                    for (int y = pt.y - 1; y <= pt.y + 1; y++)
                        for (int z = pt.z - 1; z <= pt.z + 1; z++)
                            if (x != pt.x || y != pt.y || z != pt.z)
                                yield return (x, y, z);
            }

            return Emulate(activeCells, rounds, Neighbours);
        }

        private long GoL4D(string initial, int rounds)
        {
            var activeCells = new HashSet<(int x, int y, int z, int w)>(ReadActive(initial).Select(c => (c.x, c.y, 0, 0)));

            IEnumerable<(int x, int y, int z, int w)> Neighbours((int x, int y, int z, int w) pt)
            {
                for (int x = pt.x - 1; x <= pt.x + 1; x++)
                    for (int y = pt.y - 1; y <= pt.y + 1; y++)
                        for (int z = pt.z - 1; z <= pt.z + 1; z++)
                            for (int w = pt.w - 1; w <= pt.w + 1; w++)
                                if (x != pt.x || y != pt.y || z != pt.z || w != pt.w)
                                    yield return (x, y, z, w);
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
}
