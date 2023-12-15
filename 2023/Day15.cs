namespace AdventOfCode._2023;

using System.Collections.Specialized;
using static Parser;

class Day15 : Solution
{
    private static int Hash(string input)
        => input.Aggregate(0, (a, v) => (a + v) * 17 % 256);

    private static long HashMap(string input)
    {
        var boxes = Enumerable.Range(0, 256).Select(_ => new OrderedDictionary()).ToArray();
        void Remove(string l, int v) => boxes[Hash(l)].Remove(l);
        void Add(string l, int v) => boxes[Hash(l)][l] = v;
        (
            from Label in Word
            from Op in Char('-', Remove) | Char('=', Add)
            from Value in Int.Opt()
            select new Action(() => Op(Label, Value))
        ).List(',').MustParse(input).ForEach(instruction => instruction());

        return boxes.Select((b, iB) => b.Values.Cast<int>().Select((n, iN) => n * (iN + 1)).Sum() * (iB + 1)).Sum();
    }

    protected override long? Part1()
    {
        Assert(Hash("HASH"), 52);
        Assert(Sample().Split(',').Sum(Hash), 1320);
        return Input.Split(',').Sum(Hash);
    }

    protected override long? Part2()
    {
        Assert(HashMap(Sample()), 145);
        return HashMap(Input);
    }
}
