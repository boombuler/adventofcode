namespace AdventOfCode.Utils;

using System.Numerics;
using System.Runtime.CompilerServices;

class Mat3<T> where T : INumber<T>
{
    private readonly T[] M;

    public T this[int col, int row]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => M[row * 3 + col];
    }

    private Mat3(params T[] m) => M = m;

    public static readonly Mat3<T> Identity = new(
        T.One, T.Zero, T.Zero,
        T.Zero, T.One, T.Zero,
        T.Zero, T.Zero, T.One
    );

    public Mat3<T> Translate(Point2D<T> offset)
        => this * new Mat3<T>(
            T.One, T.Zero, offset.X,
            T.Zero, T.One, offset.Y,
            T.Zero, T.Zero, T.One
        );

    public Mat3<T> FlipX()
        => this * new Mat3<T>(
            -T.One, T.Zero, T.Zero,
            T.Zero, T.One, T.Zero,
            T.Zero, T.Zero, T.One
        );

    public Mat3<T> FlipY()
        => this * new Mat3<T>(
            T.One, T.Zero, T.Zero,
            T.Zero, -T.One, T.Zero,
            T.Zero, T.Zero, T.One
        );

    public static Mat3<T> operator *(Mat3<T> m1, Mat3<T> m2) => new(
        m1[0, 0] * m2[0, 0] + m1[1, 0] * m2[0, 1] + m1[2, 0] * m2[0, 2],
        m1[0, 0] * m2[1, 0] + m1[1, 0] * m2[1, 1] + m1[2, 0] * m2[1, 2],
        m1[0, 0] * m2[2, 0] + m1[1, 0] * m2[2, 1] + m1[2, 0] * m2[2, 2],
        m1[0, 1] * m2[0, 0] + m1[1, 1] * m2[0, 1] + m1[2, 1] * m2[0, 2],
        m1[0, 1] * m2[1, 0] + m1[1, 1] * m2[1, 1] + m1[2, 1] * m2[1, 2],
        m1[0, 1] * m2[2, 0] + m1[1, 1] * m2[2, 1] + m1[2, 1] * m2[2, 2],
        m1[0, 2] * m2[0, 0] + m1[1, 2] * m2[0, 1] + m1[2, 2] * m2[0, 2],
        m1[0, 2] * m2[1, 0] + m1[1, 2] * m2[1, 1] + m1[2, 2] * m2[1, 2],
        m1[0, 2] * m2[2, 0] + m1[1, 2] * m2[2, 1] + m1[2, 2] * m2[2, 2]
    );

    public Point2D<T> Apply(Point2D<T> value)
        => (
            this[0, 0] * value.X + this[1, 0] * value.Y + this[2, 0],
            this[0, 1] * value.X + this[1, 1] * value.Y + this[2, 1]
        );

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int row = 0; row < 3; row++)
        {
            if (row > 0)
                sb.AppendLine();
            sb.Append('|');
            for (int col = 0; col < 3; col++)
            {
                sb.AppendFormat("{0,5}", this[col, row]);
            }
            sb.Append('|');
        }
        return sb.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Mat3<T> other)
            return other.M.SequenceEqual(M);
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            foreach (var item in M)
                hash = hash * 23 + item.GetHashCode();
            return hash;
        }
    }
}
