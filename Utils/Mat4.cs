using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdventOfCode.Utils
{
    class Mat4
    {
        private long[] M;

        
        public long this[int col, int row]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => M[row * 4 + col];
        }

        private Mat4(params long[] m)  => M = m;


        public static Mat4 Identity = new Mat4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        public Mat4 Translate(Point3D offset)
            => this * new Mat4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                offset.X, offset.Y, offset.Z, 1
            );

        public Mat4 Rotate90Degree(Point3D a)
        {
            var (x, y, z) = (Math.Sign(a.X), Math.Sign(a.Y), Math.Sign(a.Z));
            return this * new Mat4(
                  x * x  , y * x - z, z * x + y, 0,
                x * y + z,   y * y  , z * y - x, 0,
                z * x - y, z * y + x,   z * z  , 0,
                    0,         0,         0,     1
            );
        }

        public Point3D Apply(Point3D pt) => (
            this[0, 0] * pt.X + this[0, 1] * pt.Y + this[0, 2] * pt.Z + this[0, 3],
            this[1, 0] * pt.X + this[1, 1] * pt.Y + this[1, 2] * pt.Z + this[1, 3],
            this[2, 0] * pt.X + this[2, 1] * pt.Y + this[2, 2] * pt.Z + this[2, 3]
        );

        public Mat4 Combine(Mat4 other)
            => this * other;

        public static Mat4 operator *(Mat4 m1, Mat4 m2) => new Mat4(
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
            if (obj is Mat4 other)
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
}
