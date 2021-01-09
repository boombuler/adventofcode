using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Utils
{
    public class GameOfLife
    {
        public delegate IEnumerable<T> GetNeighbours<T>(T item);
        public delegate bool CheckAlive(bool wasAlive, int activeNeighbours);

        public static long Emulate<T>(HashSet<T> activeCells, int rounds, GetNeighbours<T> getNeighbours, CheckAlive checkAlive)
        {
            var nextGen = new HashSet<T>();
            for (int gen = 0; gen < rounds; gen++)
            {
                var lookAt = activeCells.SelectMany(t => getNeighbours(t)).Union(activeCells).Distinct().ToList();
                foreach (var cell in lookAt)
                {
                    var active = getNeighbours(cell).Where(activeCells.Contains).Count();
                    bool wasActive = activeCells.Contains(cell);

                    if (checkAlive(wasActive, active))
                        nextGen.Add(cell);
                }

                (activeCells, nextGen) = (nextGen, activeCells);
                nextGen.Clear();
            }
            return activeCells.Count();
        }

    }
}
