namespace AdventOfCode._2023;

class Day15 : Solution
{
    private static int Hash(string input)
        => input.Aggregate(0, (a, v) => (a + v) * 17 % 256);

    private static long HashMap(string input)
    {
        var boxes = Enumerable.Range(0, 256).Select(_ => new List<(string Key, int Value)>()).ToArray();
        foreach(var instr in input.Split(','))
        {
            var iOp = instr.IndexOfAny(['-', '=']);
            var label = instr[..iOp];
            var box = boxes[Hash(label)];
            var i = box.FindIndex(n => n.Key == label);
            (string, int) KeyValuePair() => (label, int.Parse(instr[(iOp + 1)..]));
            switch (instr[iOp])
            {
                case '-' when i >= 0: 
                    box.RemoveAt(i); break;
                case '=' when i >= 0:
                    box[i] = KeyValuePair(); break;
                case '=':
                    box.Add(KeyValuePair()); break;
            }
        }
        return boxes.Select((b, iB) => b.Select((n, iN) => n.Value * (iN + 1)).Sum() * (iB + 1)).Sum();
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
