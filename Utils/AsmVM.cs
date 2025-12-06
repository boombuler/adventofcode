namespace AdventOfCode.Utils;

abstract class AsmVM<TOpCode>(string code)
    where TOpCode : struct
{
    private readonly List<AsmOperation<TOpCode>> fOpCodes = [.. code.Lines().Select(o => AsmOperation<TOpCode>.Parse(o))];
    private readonly Dictionary<string, long> fRegisters = new(StringComparer.OrdinalIgnoreCase);
    private AsmOperation<TOpCode>? CurOpCode => !Finished ? fOpCodes[PC] : null;
    protected int PC { get; set; } = 0;
    protected bool Finished => PC < 0 || PC >= fOpCodes.Count;
    protected long X
    {
        get
        {
            if (long.TryParse(CurOpCode?.X, out long val))
                return val;
            return this[CurOpCode?.X];
        }
        set
        {
            this[CurOpCode?.X ?? throw new InvalidOperationException()] = value;
        }
    }

    protected long Y
    {
        get
        {
            if (long.TryParse(CurOpCode?.Y ?? string.Empty, out long val))
                return val;
            return this[CurOpCode?.Y];
        }
        set => this[CurOpCode?.Y] = value;
    }

    protected TOpCode OpCode => CurOpCode?.Kind ?? throw new InvalidOperationException();

    protected long this[string? register]
    {
        get => fRegisters.GetValueOrDefault(register ?? string.Empty);
        set => fRegisters[register ?? string.Empty] = value;
    }
}
