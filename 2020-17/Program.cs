using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace _2020_17
{
    class Program : ProgramBase
    {
        private long Emulate<T>(HashSet<T> activeCells, int rounds, Func<T, IEnumerable<T>> getNeighbours)
        {
            var nextGen = new HashSet<T>();
            for (int gen = 0; gen < rounds; gen++)
            {
                var lookAt = activeCells.SelectMany(getNeighbours).Union(activeCells).Distinct().ToList();
                foreach (var cell in lookAt)
                {
                    var active = getNeighbours(cell).Where(activeCells.Contains).Count();
                    bool wasActive = activeCells.Contains(cell);

                    if (active == 3 || (wasActive && active == 2))
                        nextGen.Add(cell);
                }

                (activeCells, nextGen) = (nextGen, activeCells);
                nextGen.Clear();
            }
            return activeCells.Count();
        }

        private IEnumerable<(int x, int y)> ReadActive(string initialFile)
        {
            int y = 0;
            foreach (var line in ReadLines(initialFile))
            {
                for (int x = 0; x < line.Length; x++)
                    if (line[x] == '#')
                        yield return (x, y);
                y++;
            }
        }
        
        
        private long GoL3D(string initialFile, int rounds)
        {
            var activeCells = new HashSet<(int x, int y, int z)>(ReadActive(initialFile).Select(c => (c.x, c.y, 0)));

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

        private long GoL4D(string initialFile, int rounds)
        {
            var activeCells = new HashSet<(int x, int y, int z, int w)>(ReadActive(initialFile).Select(c => (c.x, c.y, 0, 0)));

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

        static void Main(string[] args) => new Program().Run();

        protected override long? Part1()
        {
            Assert(GoL3D("Sample.txt", 6), 112);
            return GoL3D("Input.txt", 6);
        }

        protected override long? Part2()
        {
            Assert(GoL4D("Sample.txt", 6), 848);
            return GoL4D("Input.txt", 6);
        }
    }
}
