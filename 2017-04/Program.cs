using AdventHelper;
using System;
using System.Linq;

namespace _2017_04
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        protected override long? Part1()
            => ReadLines("Input").Select(l => l.Split(' '))
                .Where(words => words.Length == words.Distinct().Count())
                .Count();

        protected override long? Part2()
            => ReadLines("Input")
                .Select(l => l.Split(' ')
                    .Select(w => new string(w.OrderBy(c => c).ToArray()))
                    .ToArray()
                )
                .Where(words => words.Length == words.Distinct().Count())
                .Count();
    }
}
