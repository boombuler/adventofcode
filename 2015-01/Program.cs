using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2015_01
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

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

        protected override long? Part1()
        {
            var input = ReadLines("Input").First();
            return WalkLevels(input).Last();
        }

        protected override long? Part2()
        {
            var input = ReadLines("Input").First();
            return WalkLevels(input)
                .Select((l, i) => new { Level = l, Index = i })
                .Where(itm => itm.Level < 0)
                .Select(itm => itm.Index+1)
                .First();
        }
    }
}
