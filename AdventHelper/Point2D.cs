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
        public static Point2D operator *(Point2D a, long b)
            => new Point2D(a.X * b, a.Y * b);
        public IEnumerable<Point2D> Neighbours(bool withDiagonal = false)
        {
            yield return this + (1, 0);
            yield return this + (0, 1);
            yield return this - (1, 0);
            yield return this - (0, 1);
            if (withDiagonal)
            {
                yield return this + (+1, +1);
                yield return this + (-1, +1);
                yield return this + (+1, -1);
                yield return this + (-1, -1);
            }
        }

        public long ManhattanDistance(Point2D other)
            => Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
        public double Length => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));

        public static IEnumerable<Point2D> Range(Point2D min, Point2D max)
        {
            (var minX, var maxX) = min.X < max.X ? (min.X, max.X) : (max.X, min.X);
            (var minY, var maxY) = min.Y < max.Y ? (min.Y, max.Y) : (max.Y, min.Y);
            for (long x = minX; x <= maxX; x++)
                for (long y = minY; y <= maxY; y++)
                    yield return new Point2D(x, y);
        }
    }
}
