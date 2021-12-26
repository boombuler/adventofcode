namespace AdventOfCode._2016;

using System.Collections.Generic;
using System.Linq;

class Day18 : Solution
{
    private static IEnumerable<string> GenerateTiles(string firstRow)
    {
        yield return firstRow;

        var nextRow = new char[firstRow.Length];
        while (true)
        {
            nextRow[0] = firstRow[1];
            nextRow[^1] = firstRow[^2];
            for (int i = 1; i < nextRow.Length - 1; i++)
                nextRow[i] = firstRow[i - 1] != firstRow[i + 1] ? '^' : '.';
            yield return (firstRow = new string(nextRow));
        }
    }

    private static long CountSafeTiles(string firstRow, int numberOfRows)
        => GenerateTiles(firstRow).Take(numberOfRows).SelectMany(t => t).LongCount(c => c == '.');

    protected override long? Part1()
    {
        Assert(CountSafeTiles(".^^.^.^^^^", 10), 38);
        return CountSafeTiles(Input, 40);
    }

    protected override long? Part2() => CountSafeTiles(Input, 400_000);
}
