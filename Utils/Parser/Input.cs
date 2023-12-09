namespace AdventOfCode.Utils.Parser;

readonly ref struct Input(ReadOnlySpan<char> input)
{
    private readonly ReadOnlySpan<char> fSource = input;

    public readonly Input Next()
    {
        if (!EOF) 
            return new Input(fSource[1..]);
        throw new InvalidOperationException("No more Data");
    }

    public readonly Input Seek(int offset)
        => new(fSource[offset..]);

    public readonly char Current => fSource[0];

    public readonly bool EOF => fSource.Length == 0;

    public readonly string Remaining => new(fSource);
}
