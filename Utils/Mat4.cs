namespace AdventOfCode.Utils;

using System.Numerics;
using System.Runtime.CompilerServices;

class Mat4<T> where T : INumber<T>
{
    private readonly T[] M;

    public T this[int col, int row]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => M[row * 4 + col];
    }

    private Mat4(params T[] m) => M = m;

    public static Mat4<T> Identity = new(
        T.One, T.Zero, T.Zero, T.Zero,
        T.Zero, T.One, T.Zero, T.Zero,
        T.Zero, T.Zero, T.One, T.Zero,
        T.Zero, T.Zero, T.Zero, T.One
    );

    public Mat4<T> Translate(Point3D<T> offset)
        => this * new Mat4<T>(
            T.One, T.Zero, T.Zero, T.Zero,
            T.Zero, T.One, T.Zero, T.Zero,
            T.Zero, T.Zero, T.One, T.Zero,
            offset.X, offset.Y, offset.Z, T.One
        );

    public Mat4<T> Rotate90Degree(Point3D<T> a)
    {
        var (x, y, z) = (Sign(a.X), Sign(a.Y), Sign(a.Z));
        return this * new Mat4<T>(
            x * x, y * x - z, z * x + y, T.Zero,
            x * y + z, y * y, z * y - x, T.Zero,
            z * x - y, z * y + x, z * z, T.Zero,
            T.Zero, T.Zero, T.Zero, T.One
        );
    }

    private static T Sign(T v)
    {
        if (v > T.Zero)
            return T.One;
        if (v < T.Zero)
            return -T.One;
        return T.Zero;
    }

    public Point3D<T> Apply(Point3D<T> pt) => (
        this[0, 0] * pt.X + this[0, 1] * pt.Y + this[0, 2] * pt.Z + this[0, 3],
        this[1, 0] * pt.X + this[1, 1] * pt.Y + this[1, 2] * pt.Z + this[1, 3],
        this[2, 0] * pt.X + this[2, 1] * pt.Y + this[2, 2] * pt.Z + this[2, 3]
    );

    public Mat4<T> Combine(Mat4<T> other)
        => this * other;

    public static Mat4<T> operator *(Mat4<T> m1, Mat4<T> m2) => new(
        m1[0, 0] * m2[0, 0] + m1[0, 1] * m2[1, 0] + m1[0, 2] * m2[2, 0] + m1[0, 3] * m2[3, 0],
        m1[1, 0] * m2[0, 0] + m1[1, 1] * m2[1, 0] + m1[1, 2] * m2[2, 0] + m1[1, 3] * m2[3, 0],
        m1[2, 0] * m2[0, 0] + m1[2, 1] * m2[1, 0] + m1[2, 2] * m2[2, 0] + m1[2, 3] * m2[3, 0],
        m1[3, 0] * m2[0, 0] + m1[3, 1] * m2[1, 0] + m1[3, 2] * m2[2, 0] + m1[3, 3] * m2[3, 0],
        m1[0, 0] * m2[0, 1] + m1[0, 1] * m2[1, 1] + m1[0, 2] * m2[2, 1] + m1[0, 3] * m2[3, 1],
        m1[1, 0] * m2[0, 1] + m1[1, 1] * m2[1, 1] + m1[1, 2] * m2[2, 1] + m1[1, 3] * m2[3, 1],
        m1[2, 0] * m2[0, 1] + m1[2, 1] * m2[1, 1] + m1[2, 2] * m2[2, 1] + m1[2, 3] * m2[3, 1],
        m1[3, 0] * m2[0, 1] + m1[3, 1] * m2[1, 1] + m1[3, 2] * m2[2, 1] + m1[3, 3] * m2[3, 1],
        m1[0, 0] * m2[0, 2] + m1[0, 1] * m2[1, 2] + m1[0, 2] * m2[2, 2] + m1[0, 3] * m2[3, 1],
        m1[1, 0] * m2[0, 2] + m1[1, 1] * m2[1, 2] + m1[1, 2] * m2[2, 2] + m1[1, 3] * m2[3, 1],
        m1[2, 0] * m2[0, 2] + m1[2, 1] * m2[1, 2] + m1[2, 2] * m2[2, 2] + m1[2, 3] * m2[3, 1],
        m1[3, 0] * m2[0, 2] + m1[3, 1] * m2[1, 2] + m1[3, 2] * m2[2, 2] + m1[3, 3] * m2[3, 1],
        m1[0, 0] * m2[0, 3] + m1[0, 1] * m2[1, 3] + m1[0, 2] * m2[2, 3] + m1[0, 3] * m2[3, 3],
        m1[1, 0] * m2[0, 3] + m1[1, 1] * m2[1, 3] + m1[1, 2] * m2[2, 3] + m1[1, 3] * m2[3, 3],
        m1[2, 0] * m2[0, 3] + m1[2, 1] * m2[1, 3] + m1[2, 2] * m2[2, 3] + m1[2, 3] * m2[3, 3],
        m1[3, 0] * m2[0, 3] + m1[3, 1] * m2[1, 3] + m1[3, 2] * m2[2, 3] + m1[3, 3] * m2[3, 3]
    );

    public override bool Equals(object obj)
    {
        if (obj is Mat4<T> other)
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

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int row = 0; row < 4; row++)
        {
            if (row > 0)
                sb.AppendLine();
            sb.Append('|');
            for (int col = 0; col < 4; col++)
            {
                sb.AppendFormat("{0,5}", this[col, row]);
            }
            sb.Append('|');
        }
        return sb.ToString();
    }
}
