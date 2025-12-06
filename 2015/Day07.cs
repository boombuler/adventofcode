namespace AdventOfCode._2015;

class Day07 : Solution
{
    enum Operation
    {
        Intermediate, AND, OR, NOT, LSHIFT, RSHIFT
    }
    record Instruction(string Arg0, string Wire, string? Arg1 = null, Operation Op = Operation.Intermediate)
    {
        public static Func<string, Instruction?> Parse = new Regex(@"(((?<Arg1>(\d+|\w+))\s)?(?<Op>NOT|RSHIFT|LSHIFT|OR|AND)\s)?(?<Arg0>\d+|\w+)\s->\s(?<Wire>\w+)").ToFactory<Instruction>();
    }

    private static ushort TestWire(string instructions, string wire, (string wire, ushort value)? wireOverride = null)
    {
        var wireNet = new Dictionary<string, Lazy<ushort>>();

        Func<ushort> ValueOf(string? v)
        {
            if (v == null) 
                return () => throw new InvalidInputException();
            if (ushort.TryParse(v, out ushort val))
                return () => val;
            return () => wireNet[v].Value;
        }
            

        foreach (var instruction in instructions.Lines().Select(Instruction.Parse))
        {
            if (instruction == null)
                throw new InvalidInputException();
            var arg0 = ValueOf(instruction.Arg0);
            var arg1 = ValueOf(instruction.Arg1);
            wireNet[instruction.Wire] = new Lazy<ushort>(instruction.Op switch {
                Operation.AND => () => (ushort)(arg1() & arg0()),
                Operation.OR => () => (ushort)(arg1() | arg0()),
                Operation.NOT => () => (ushort)(~arg0()),
                Operation.LSHIFT => () => (ushort)(arg1() << arg0()),
                Operation.RSHIFT => () => (ushort)(arg1() >> arg0()),
                _ => arg0
            });
        }
        if (wireOverride is (string w, ushort v))
            wireNet[w] = new Lazy<ushort>(() => v);

        return wireNet[wire].Value;
    }

    protected override long? Part1()
    {
        Assert(TestWire(Sample(), "d"), 72, "d");
        Assert(TestWire(Sample(), "e"), 507, "e");
        Assert(TestWire(Sample(), "f"), 492, "f");
        Assert(TestWire(Sample(), "g"), 114, "g");
        Assert(TestWire(Sample(), "h"), 65412, "h");
        Assert(TestWire(Sample(), "i"), 65079, "i");
        Assert(TestWire(Sample(), "x"), 123, "x");
        Assert(TestWire(Sample(), "y"), 456, "y");
        return TestWire(Input, "a");
    }
    protected override long? Part2()
    {
        var oldA = TestWire(Input, "a");
        return TestWire(Input, "a", ("b", oldA));
    }
}
