using System;
using System.Collections.Generic;
using System.Text;

namespace AdventHelper
{
    public struct Point2D
    {
        public long X { get; private set; }
        public long Y { get; private set; }

        public Point2D(long x, long y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point2D p)
                return p.X == X && p.Y == Y;
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 17;
                result = (result * 23) + X.GetHashCode();
                result = (result * 23) + Y.GetHashCode();
                return result;
            }
        }

        public override string ToString() => $"{X}|{Y}";

        public void Deconstruct(out long x, out long y)
        {
            x = X; y = Y;
        }
        public static implicit operator (long, long)(Point2D pt) => (pt.X, pt.Y);
        public static implicit operator Point2D((long, long) t) => new Point2D(t.Item1, t.Item2);

        public static Point2D operator -(Point2D a, Point2D b)
            => new Point2D(a.X - b.X, a.Y - b.Y);
        public static Point2D operator +(Point2D a, Point2D b)
            => new Point2D(a.X + b.X, a.Y + b.Y);

        public double Length => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
    }
}
