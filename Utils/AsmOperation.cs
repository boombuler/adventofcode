namespace AdventOfCode.Utils;

record AsmOperation<TOpCode>(TOpCode Kind, string X, string Y) where TOpCode : struct
{
    private static readonly char[] Separators = [' ', ','];
    public static AsmOperation<TOpCode> Parse(string line)
    {
        var parts = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
        return new AsmOperation<TOpCode>(
            Enum.Parse<TOpCode>(parts[0]),
            (parts.Length > 1) ? parts[1] : null,
            (parts.Length > 2) ? parts[2] : null);
    }
}
