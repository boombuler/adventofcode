namespace AdventOfCode.Utils.Parser;

ref struct Input
{
    private readonly ReadOnlySpan<char> fSource;
    private readonly int fPos;
    
    public Input(ReadOnlySpan<char> input)
    {
        fSource = input;
    }

    public Input Next()
    {
        if (!EOF) 
            return new Input(fSource.Slice(1));
        throw new InvalidOperationException("No more Data");
    }

    public Input Seek(int offset)
        => new Input(fSource.Slice(offset));

    public char Current => fSource[fPos];

    public bool EOF => fSource.Length == 0;

    public string Remaining => new string(fSource);
}
