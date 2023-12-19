namespace AdventOfCode.Utils;

using System.Diagnostics;
using System.Numerics;

[DebuggerDisplay("[{Start}, {End})")]
record struct Range<T>(T Start, T Size) where T: INumber<T>
{
    /// <summary>
    /// Exclusive End of the Range
    /// </summary>
    public readonly T End => Start + Size;

    public Range<T> Intersect(Range<T> other, out IEnumerable<Range<T>> unintersected)
    {
        var olStart = T.Max(Start, other.Start);
        var olEnd = T.Min(End, other.End);
        if (olEnd < olStart)
        {
            unintersected = [this];
            return new Range<T>(T.Zero, T.Zero);
        }
        unintersected = new[] {
                new Range<T>(Start, olStart - Start),
                new Range<T>(olEnd, End - olEnd)
            }.Where(i => i.Size > T.Zero);
        return new Range<T>(olStart, olEnd - olStart);
    }

    public readonly bool Contains(T value)
        => value >= Start && value < End;
}