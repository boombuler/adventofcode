namespace AdventOfCode._2015;

class Day20 : Solution
{
    private static int FindMinHouse(int minPresents, int presentsPerHouse, int? maxPerElv)
    {
        var size = minPresents / presentsPerHouse;
        var counts = new int[size];
        for (int e = 1; e < size; e++)
        {
            var max = maxPerElv.HasValue ? Math.Min(e * maxPerElv.Value, size) : size;
            for (int i = e; i < max; i += e)
                counts[i] += (presentsPerHouse * e);
        }

        for (int i = 0; i < size; i++)
            if (counts[i] >= minPresents)
                return i;
        return 0;
    }

    private static long MinHouseNoEndlessHouses(int presents)
        => FindMinHouse(presents, 10, null);

    private static long MinHouseNo(int target)
        => FindMinHouse(target, 11, 50);

    protected override long? Part1()
    {
        Assert(MinHouseNoEndlessHouses(150), 8);
        return MinHouseNoEndlessHouses(int.Parse(Input));
    }

    protected override long? Part2()
        => MinHouseNo(int.Parse(Input));
}
