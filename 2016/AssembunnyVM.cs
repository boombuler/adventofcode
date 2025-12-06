namespace AdventOfCode._2016;

using System.Threading.Channels;

class AssembunnyVM
{
    record Register(int Index);

    private static Action<AssembunnyVM> CompileOp(object[] args)
        => args switch
        {
            ["cpy", var x, Register y] => vm => vm.Registers[y.Index] = vm[x],
            ["inc", Register x] => vm => vm[x]++,
            ["dec", Register x] => vm => vm[x]--,
            ["jnz", var x, var y] => vm => vm.PC += (int)(vm[x] != 0 ? vm[y] - 1 : 0),
            ["tgl", var x] => vm => vm.Toggle((int)vm[x]),
            ["mul", Register x, var y] => vm => vm[x] = vm[x] * vm[y],
            ["out", var x] => vm => vm.fOutput.Writer.TryWrite(vm[x]),
            _ => _ => { /* nop */ }
        };
    private static Action<AssembunnyVM> CompileToggledOp(object[] args)
        => args switch
        {
            ["inc", var x] => CompileOp(["dec", x]),
            [_, var x] => CompileOp(["inc", x]),
            ["jnz", var x, var y] => CompileOp(["cpy", x, y]),
            [_, var x, var y] => CompileOp(["jnz", x, y]),
            ["nop"] => CompileOp(args),
            ["mul"] => CompileOp(args),
            _ => throw new InvalidOperationException()
        };

    private readonly Channel<long> fOutput = Channel.CreateUnbounded<long>();
    private readonly HashSet<int> fToggled = [];
    private readonly Action<AssembunnyVM>[] fNormalOperations;
    private readonly Action<AssembunnyVM>[] fToggledOperations;
    protected readonly long[] Registers = new long[4];
    protected int PC;

    protected ChannelReader<long> Output => fOutput.Reader;
    
    private long this[Register r]
    {
        get => Registers[r.Index];
        set => Registers[r.Index] = value;
    }

    private long this[object o] => o switch
    {
        Register r => Registers[r.Index],
        long l => l,
        _ => throw new InvalidOperationException()
    };

    protected bool Finished => PC >= fNormalOperations.Length;

    public AssembunnyVM(string code)
    {
        var args = OptimizeMultiplication(code).Lines().Select(GetArgs).ToList();
        fNormalOperations = [.. args.Select(CompileOp)];
        fToggledOperations = [.. args.Select(CompileToggledOp)];
    }

    public long Run(long a = 0, long b = 0, long c = 0, long d = 0)
    {
        Reset();

        Registers[0] = a;
        Registers[1] = b;
        Registers[2] = c;
        Registers[3] = d;
        
        while (!Finished)
        {
            Operation(this);
            PC++;
        }
        return Registers[0];
    }

    protected Action<AssembunnyVM> Operation
        => (fToggled.Contains(PC) ? fToggledOperations : fNormalOperations)[PC];

    protected void Reset()
    {
        fToggled.Clear();
        PC = 0;
    }

    private static object[] GetArgs(string s)
    => [.. s.Split(' ').Select<string, object>((s, i) => (s, i) switch
    {
        (_, 0) => s,
        (string a, _) when long.TryParse(a, out long arg) => arg,
        (string a, _) when a.Length == 1 => new Register(a[0] - 'a'),
        _ => throw new InvalidOperationException()
    })];

    private void Toggle(int offset)
    {
        int line = offset + PC;
        if (!fToggled.Add(line))
            fToggled.Remove(line);
    }

    private static string OptimizeMultiplication(string input)
    {
        const string PATTERN = @"cpy a d
cpy 0 a
cpy b c
inc a
dec c
jnz c -2
dec d
jnz d -5";
        const string REPLACEMENT = @"mul a b
nop
nop
nop
nop
nop
nop
nop";
        return input.Replace(PATTERN.ReplaceLineEndings("\n"), REPLACEMENT);
    }
}
