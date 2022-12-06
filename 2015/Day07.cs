namespace AdventOfCode._2015;

class Day07 : Solution
{
    enum Operation
    {
        Intermediate, AND, OR, NOT, LSHIFT, RSHIFT
    }
    record Instruction(string Arg0, string Wire, string Arg1 = null, Operation Op = Operation.Intermediate)
    {
        public static Func<string, Instruction> Parse = new Regex(@"(((?<Arg1>(\d+|\w+))\s)?(?<Op>NOT|RSHIFT|LSHIFT|OR|AND)\s)?(?<Arg0>\d+|\w+)\s->\s(?<Wire>\w+)").ToFactory<Instruction>();
    }

    private static ushort TestWire(string instructions, string wire, (string wire, ushort value)? wireOverride = null)
    {
        var wireNet = new Dictionary<string, Func<ushort>>();

        Func<ushort> ValueOf(string v)
        {
            v ??= string.Empty;
            if (ushort.TryParse(v, out ushort val))
                return () => val;
            return () => wireNet[v]();
        }

        foreach (var instruction in instructions.Lines().Select(Instruction.Parse))
        {
            void CalcLazy(Func<ushort> generator)
            {
                var val = new Lazy<ushort>(generator);
                wireNet[instruction.Wire] = () => val.Value;
            }

            var arg0 = ValueOf(instruction.Arg0);
            var arg1 = ValueOf(instruction.Arg1);

            switch (instruction.Op)
            {
                case Operation.Intermediate:
                    CalcLazy(arg0); break;
                case Operation.AND:
                    CalcLazy(() => (ushort)(arg1() & arg0())); break;
                case Operation.OR:
                    CalcLazy(() => (ushort)(arg1() | arg0())); break;
                case Operation.NOT:
                    CalcLazy(() => (ushort)(~arg0())); break;
                case Operation.LSHIFT:
                    CalcLazy(() => (ushort)(arg1() << arg0())); break;
                case Operation.RSHIFT:
                    CalcLazy(() => (ushort)(arg1() >> arg0())); break;
            }
        }
        if (wireOverride.HasValue)
            wireNet[wireOverride.Value.wire] = () => wireOverride.Value.value;

        return wireNet[wire]();
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
