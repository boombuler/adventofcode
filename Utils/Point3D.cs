using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Utils
{
    public struct Point3D
    {
        public long X { get; private set; }
        public long Y { get; private set; }
        public long Z { get; private set; }

        public Point3D(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point3D p)
                return p.X == X && p.Y == Y && p.Z == Z;
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 17;
                result = (result * 23) + X.GetHashCode();
                result = (result * 23) + Y.GetHashCode();
                result = (result * 23) + Z.GetHashCode();
                return result;
            }
        }

        public override string ToString() => $"{X}|{Y}|{Z}";

        public void Deconstruct(out long x, out long y, out long z)
        {
            x = X; y = Y; z = Z;
        }
        public static implicit operator (long, long, long)(Point3D pt) => (pt.X, pt.Y,pt.Z);
        public static implicit operator Point3D((long, long, long) t) => new Point3D(t.Item1, t.Item2, t.Item3);

        public static Point3D operator -(Point3D a, Point3D b)
            => new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Point3D operator +(Point3D a, Point3D b)
            => new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Point3D operator *(Point3D a, long b)
            => new Point3D(a.X * b, a.Y * b, a.Z * b);
        
        public long ManhattanDistance(Point3D other)
            => Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);
        public double Length => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));

    }
}
