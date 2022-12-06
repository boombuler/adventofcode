namespace AdventOfCode.Utils;

ref struct AsciiCounter
{
    private readonly Span<byte> fBuffer;
    private int fLength;
    public int Length => fLength;
    public long Value => long.Parse(Encoding.ASCII.GetString(fBuffer[..Length]));
    public AsciiCounter(Span<byte> buffer, long initialValue = 0)
    {
        fBuffer = buffer;
        var b = Encoding.ASCII.GetBytes(initialValue.ToString());
        b.CopyTo<byte>(fBuffer);
        fLength = b.Length;
    }

    public void Step() => Step(fBuffer, ref fLength);

    public static void Step(Span<byte> buffer, ref int Length)
    {
        var idx = Length - 1;
        while (true)
        {
            buffer[idx]++;
            if (buffer[idx] > (byte)'9')
            {
                buffer[idx--] = (byte)'0';
                if (idx < 0)
                {
                    for (int i = Length; i > 0; i--)
                        buffer[i] = buffer[i - 1];
                    buffer[0] = (byte)'0';
                    idx = 0;
                    Length++;
                }
            }
            else
                return;
        }
    }
}
