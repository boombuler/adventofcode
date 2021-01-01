using System;
using System.Collections.Generic;
using System.Linq;
using AdventHelper;

namespace _2016_18
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private IEnumerable<string> GenerateTiles(string firstRow)
        {
            yield return firstRow;

            var nextRow = new char[firstRow.Length];
            while (true)
            {
                nextRow[0] = firstRow[1];
                nextRow[nextRow.Length - 1] = firstRow[firstRow.Length - 2];
                for (int i = 1; i < nextRow.Length-1; i++)
                    nextRow[i] = firstRow[i - 1] != firstRow[i + 1] ? '^' : '.';
                yield return (firstRow = new string(nextRow));
            }
        }

        private long CountSafeTiles(string firstRow, int numberOfRows)
            => GenerateTiles(firstRow).Take(numberOfRows).SelectMany(t => t).LongCount(c => c == '.');

        const string INPUT = ".^^^^^.^^^..^^^^^...^.^..^^^.^^....^.^...^^^...^^^^..^...^...^^.^.^.......^..^^...^.^.^^..^^^^^...^.";

        protected override long? Part1()
        {
            Assert(CountSafeTiles(".^^.^.^^^^", 10), 38);
            return CountSafeTiles(INPUT, 40);
        }

        protected override long? Part2() => CountSafeTiles(INPUT, 400_000);
    }
}
