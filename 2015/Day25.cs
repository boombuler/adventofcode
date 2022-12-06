namespace AdventOfCode._2015;

class Day25 : Solution
{
    private static IEnumerable<(int column, int row)> GeneratePositions()
    {
        int startRow = 0;

        while (true)
        {
            startRow++;

            int col = 1;
            for (int row = startRow; row > 0; row--)
            {
                yield return (col++, row);
            }
        }
    }

    private static IEnumerable<long> GenerateCodes()
    {
        long value = 20151125;

        while (true)
        {
            yield return value;
            value = (value * 252533) % 33554393;
        }
    }

    private static IEnumerable<(int column, int row, long code)> GenerateTable()
        => GeneratePositions().Zip(GenerateCodes(), (pos, val) => (pos.column, pos.row, val));

    private static long LookupCode(int column, int row)
        => GenerateTable().Where(x => x.column == column && x.row == row).Select(x => x.code).First();

    protected override long? Part1()
    {
        Assert(LookupCode(4, 4), 9380097);
        return LookupCode(3019, 3010);
    }

}
