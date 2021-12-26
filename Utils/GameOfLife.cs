namespace AdventOfCode.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

public class GameOfLife
{
    public delegate IEnumerable<T> GetNeighbours<T>(T item);
    public delegate bool CheckAlive(bool wasAlive, int activeNeighbours);

    public static long Emulate<T>(HashSet<T> activeCells, int rounds, GetNeighbours<T> getNeighbours, CheckAlive checkAlive)
    {
        var nextGen = new HashSet<T>();
        var curGen = new HashSet<T>(activeCells);
        for (int gen = 0; gen < rounds; gen++)
        {
            var lookAt = curGen.SelectMany(t => getNeighbours(t)).Union(curGen).Distinct().ToList();
            foreach (var cell in lookAt)
            {
                var active = getNeighbours(cell).Where(curGen.Contains).Count();
                bool wasActive = curGen.Contains(cell);

                if (checkAlive(wasActive, active))
                    nextGen.Add(cell);
            }

            (curGen, nextGen) = (nextGen, curGen);
            nextGen.Clear();
        }
        activeCells.Clear();
        foreach (var n in curGen)
            activeCells.Add(n);
        return curGen.Count;
    }

}
