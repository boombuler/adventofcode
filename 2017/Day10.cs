namespace AdventOfCode._2017;

class Day10 : Solution<long?, string>
{
    private static int SimpleKnotHash(int listLen, IEnumerable<int> knotLengths)
    {
        List<int> list = Enumerable.Range(0, listLen).ToList();
        int current = 0, skipSize = 0;
        KnotHashRound(list, ref current, ref skipSize, knotLengths);
        return list[0] * list[1];
    }

    private static void KnotHashRound(List<int> list, ref int current, ref int skipSize, IEnumerable<int> knotLengths)
    {
        foreach (var len in knotLengths)
        {
            var rangeEnd = current + len - 1;
            for (int i = 0; i < (len / 2); i++)
            {
                int a = (current + i) % list.Count;
                int b = (rangeEnd - i) % list.Count;
                (list[a], list[b]) = (list[b], list[a]);
            }
            current = (current + len + skipSize++) % list.Count;
        }
    }

    private static string KnotHash64(string prefix)
    {
        var numbers = Enumerable.Range(0, 256).ToList();
        var lengths = Encoding.ASCII.GetBytes(prefix)
            .Select(b => (int)b)
            .Concat([17, 31, 73, 47, 23])
            .ToArray();
        int current = 0, skipSize = 0;
        for (int round = 0; round < 64; round++)
            KnotHashRound(numbers, ref current, ref skipSize, lengths);

        return string.Concat(numbers
            .Chunk(16)
            .Select(chunk => chunk.Aggregate((a, b) => a ^ b))
            .Select(val => val.ToString("x2"))
        );
    }

    protected override long? Part1()
    {
        Assert(SimpleKnotHash(5, [3, 4, 1, 5]), 12);
        return SimpleKnotHash(256, Input.Split(',').Select(int.Parse).ToArray());
    }

    protected override string Part2()
    {
        Assert(KnotHash64(string.Empty), "a2582a3a0e66e6e86e3812dcb672a272");
        Assert(KnotHash64("AoC 2017"), "33efeb34ea91902bb2f59c9920caa6cd");
        Assert(KnotHash64("1,2,3"), "3efbe78a8d82f29979031a4aa0b16a9d");
        Assert(KnotHash64("1,2,4"), "63960835bcdc130f0b66d7ff4f6a5a8e");

        return KnotHash64(Input);
    }
}
