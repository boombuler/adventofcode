namespace AdventOfCode._2018;

class Day14 : Solution<string, int>
{
    private static IEnumerable<int> Generate()
    {
        var wall = new List<int>();
        int Add(int v)
        {
            wall.Add(v);
            return v;
        }
        void Seek(ref int e) => e = (e + 1 + wall[e]) % wall.Count;

        yield return Add(3);
        yield return Add(7);
        int e0 = 0, e1 = 1;

        while (true)
        {
            var sum = wall[e0] + wall[e1];
            if (sum >= 10)
                yield return Add(1); // Since every number is a digit between 0 and 9, there will never be a number > 18. so if there is a second digit, its a 1
            yield return Add(sum % 10);
            Seek(ref e0);
            Seek(ref e1);
        }
    }

    private static string GenRecipes(int count)
        => string.Concat(Generate().Skip(count).Take(10));

    private static int FindSequence(string seqstr)
    {
        var seq = long.Parse(seqstr);
        long cap = (long)Math.Pow(10, seqstr.Length);

        int genNo = 0;
        long current = 0;
        foreach (var next in Generate())
        {
            genNo++;
            current = ((current * 10) + next) % cap;
            if (current == seq)
                return genNo - seqstr.Length;
        }
        return int.MinValue;
    }

    protected override string Part1()
    {
        Assert(GenRecipes(9), "5158916779");
        Assert(GenRecipes(5), "0124515891");
        Assert(GenRecipes(18), "9251071085");
        Assert(GenRecipes(2018), "5941429882");
        return GenRecipes(int.Parse(Input));
    }
    protected override int Part2()
    {
        Assert(FindSequence("51589"), 9);
        Assert(FindSequence("01245"), 5);
        Assert(FindSequence("92510"), 18);
        Assert(FindSequence("59414"), 2018);
        return FindSequence(Input);
    }
}
