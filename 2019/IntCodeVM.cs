namespace AdventOfCode._2019;

using System.Text;

public record IntCodeVM(long PC, long RelativeBaseOffset, ImmutableSortedDictionary<long, long> Data)
{
    private const long OC_HALT = 99;
    private const long OC_ADD = 1;
    private const long OC_MUL = 2;
    private const long OC_IN = 3;
    private const long OC_OUT = 4;
    private const long OC_JNZ = 5;
    private const long OC_JZ = 6;
    private const long OC_LT = 7;
    private const long OC_EQ = 8;
    private const long OC_ADJRBA = 9; // Adjust relative base offset.

    private const long PM_POS = 0;
    private const long PM_IMM = 1;
    private const long PM_REL = 2;

    public bool Halted => PC < 0;

    public IntCodeVM(string code)
        : this(0, 0, code.Split(',').Select(long.Parse).Select((v, i) => (v, i)).ToImmutableSortedDictionary(n => (long)n.i, n => n.v))
    {
    }

    public long this[long idx]
        => idx < 0 ? throw new InvalidOperationException() : (Data.TryGetValue(idx, out var res) ? res : 0);

    public IntCodeVM SetAddress(long address, long value)
        => this with { Data = Data.SetItem(address, value) };

    private long Param(ref long pc, ref long opcode)
    {
        var m = opcode % 10;
        opcode /= 10;

        var value = Data[pc++];
        return m switch
        {
            PM_POS => this[value],
            PM_IMM => value,
            PM_REL => this[value + RelativeBaseOffset],
            _ => throw new NotImplementedException(),
        };
    }

    private IntCodeVM SetParam(long pc, ref long opcode, long value)
    {
        var m = opcode % 10;
        opcode /= 10;

        var addr = this[pc];
        if (m == PM_REL)
            addr += RelativeBaseOffset;

        return this with { PC = pc + 1, Data = Data.SetItem(addr, value) };
    }

    private IntCodeVM BinaryOp(long pc, long p, Func<long, long, long> op)
    {
        var (a, b) = (Param(ref pc, ref p), Param(ref pc, ref p));
        return SetParam(pc, ref p, op(a, b));
    }

    private IntCodeVM ConditionalJump(long pc, long p, Predicate<long> test)
    {
        var (t, target) = (Param(ref pc, ref p), Param(ref pc, ref p));
        if (test(t))
            pc = target;
        return this with { PC = pc };
    }

    public (long? Value, IntCodeVM NextState) Step(Func<long> currentInput)
    {
        var pc = PC;
        var opCode = this[pc++];
        var p = opCode / 100;

        return (opCode % 100) switch
        {
            OC_HALT => (null, this with { PC = -1 }),
            OC_ADD => (null, BinaryOp(pc, p, (a, b) => a + b)),
            OC_MUL => (null, BinaryOp(pc, p, (a, b) => a * b)),
            OC_LT => (null, BinaryOp(pc, p, (a, b) => a < b ? 1 : 0)),
            OC_EQ => (null, BinaryOp(pc, p, (a, b) => a == b ? 1 : 0)),
            OC_OUT => (Param(ref pc, ref p), this with { PC = pc }),
            OC_IN => (null, SetParam(pc, ref p, currentInput())),
            OC_ADJRBA => (null, this with
            {
                RelativeBaseOffset = RelativeBaseOffset + Param(ref pc, ref p),
                PC = pc
            }),
            OC_JNZ => (null, ConditionalJump(pc, p, n => n != 0)),
            OC_JZ => (null, ConditionalJump(pc, p, n => n == 0)),
            _ => throw new InvalidOperationException("Unsupported OpCode")
        };
    }

    public IEnumerable<long> Run(params long[] input)
    {
        var inputEnumerator = input.GetEnumerator();
        return Run(() =>
        {
            if (!inputEnumerator.MoveNext())
                throw new InvalidOperationException("missing input");
            return (long)inputEnumerator.Current;
        });
    }

    public IEnumerable<long> Run(Func<long> currentInput)
    {
        var s = this;
        while (!s.Halted)
        {
            long? v;
            (v, s) = s.Step(currentInput);
            if (v.HasValue)
                yield return v.Value;
        }
    }

    public IEnumerable<(string Result, IntCodeVM State)> RunASCIICommands(IEnumerable<string> commands = null)
    {
        var queue = new Queue<long>();
        var sbLine = new StringBuilder();
        long? d;
        var cmds = (commands ?? Enumerable.Empty<string>()).GetEnumerator();
        var vm = this;
        const int EOL = 10;
        while (!vm.Halted)
        {
            var stop = false;

            (d, vm) = vm.Step(() =>
            {
                if (queue.Count == 0)
                {
                    if (!cmds.MoveNext())
                    {
                        stop = true;
                        return -1;
                    }
                    foreach (var c in cmds.Current)
                        queue.Enqueue(c);
                    queue.Enqueue(EOL);
                }
                return queue.Dequeue();
            });

            if (d.HasValue)
            {
                if (d == EOL)
                {
                    var str = sbLine.ToString();
                    yield return (str, vm);
                    sbLine.Clear();
                }
                else if (d.Value > 127) // treat as number
                    sbLine.Append(d.Value);
                else
                    sbLine.Append((char)d.Value);
            }

            if (stop)
                break;
        }
        if (sbLine.Length > 0)
            yield return (sbLine.ToString(), vm);
    }
}
