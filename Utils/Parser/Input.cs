namespace AdventOfCode.Utils.Parser;

class Input
{
    private readonly string fSource;
    private readonly int fPos;

    public Input(string source)
        : this(source, 0)
    {   
    }

    private Input(string source, int pos)
    {
        fSource = source;
        fPos = pos;
        if (pos < 0)
            throw new ArgumentOutOfRangeException(nameof(pos));
    }

    public Input Next()
    {
        if (!EOF) 
            return new Input(fSource, fPos+1);
        throw new InvalidOperationException("No more Data");
    }

    public Input Seek(int offset)
        => new Input(fSource, fPos+offset);

    public char Current => fSource[fPos];

    public bool EOF => fPos >= fSource.Length;

    public string Remaining => fSource.Substring(fPos);
}
