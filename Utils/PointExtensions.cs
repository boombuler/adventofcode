namespace AdventOfCode.Utils;

using System.Numerics;

static class PointExtensions
{
    extension<T>(Point3D<T> pt) where T : IRootFunctions<T>, INumber<T>
    {
        public T StraightLineDistance(Point3D<T> other)
        {
            var d = (other - pt);
            return T.Sqrt(d.X * d.X + d.Y * d.Y + d.Z * d.Z);
        }
    }
}
