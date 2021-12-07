using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2015
{
    class Day01 : Solution
    {
        public IEnumerable<int> WalkLevels(string input)
        {
            int level = 0;
            foreach(var c in input)
            {
                if (c == '(')
                    level++;
                else
                    level--;
                yield return level;
            }
        }

        protected override long? Part1() => WalkLevels(Input).Last();

        protected override long? Part2()
            => WalkLevels(Input)
                .Select((l, i) => new { Level = l, Index = i })
                .Where(itm => itm.Level < 0)
                .Select(itm => itm.Index+1)
                .First();
    }
}
