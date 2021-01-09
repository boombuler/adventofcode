using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2015
{
    class Day03 : Solution
    {

        public IEnumerable<(int x, int y)> Walk(IEnumerable<char> directions)
        {
            var current = (x: 0, y: 0);
            yield return current;
            foreach(var d in directions)
            {
                switch(d)
                {
                    case '^': current = (current.x, current.y - 1); break;
                    case 'v': current = (current.x, current.y + 1); break;
                    case '<': current = (current.x-1, current.y); break;
                    case '>': current = (current.x + 1, current.y); break;
                }
                yield return current;
            }    
        }

        public int CountUniquePlaces(string directions, int workers = 1)
            => Enumerable.Range(0, workers)
                .SelectMany(w => Walk(directions.Skip(w).Where((c, i) => (i % workers) == 0)))
                .Distinct()
                .Count();

        protected override long? Part1()
        {
            Assert(CountUniquePlaces(">"), 2);
            Assert(CountUniquePlaces("^>v<"), 4);
            Assert(CountUniquePlaces("^v^v^v^v^v"), 2);
            return CountUniquePlaces(Input);
        }

        protected override long? Part2()
        {
            Assert(CountUniquePlaces("^v", 2), 3);
            Assert(CountUniquePlaces("^>v<", 2), 3);
            Assert(CountUniquePlaces("^v^v^v^v^v", 2), 11);
            return CountUniquePlaces(Input, 2);
        }
    }
}
