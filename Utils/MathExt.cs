namespace AdventOfCode.Utils;

using System.Numerics;

class MathExt
{
    public static T GCD<T>(T a, T b) 
        where T: INumber<T>
    {
        while (b != T.Zero)
            (a, b) = (b, a % b);
        return a;
    }
    public static T LCM<T>(T a, T b)
        where T: INumber<T>
    {
        var gcd = GCD(a, b);
        return (a / gcd) * b;
    }

    public static T Mod<T>(T a, T m)
        where T : IModulusOperators<T,T,T>, IAdditionOperators<T, T, T>
        => ((a % m) + m) % m;

    /// <summary>
    /// Solves `ax + by = GCD(a, b)`
    /// </summary>
    public static (T x, T y) ExtEuclid<T>(T a, T b)
        where T : INumber<T>
    {
        T x0 = T.One;
        T y0 = T.Zero;
        T x1 = T.Zero;
        T y1 = T.One;
        T r = a % b;
        T xn = T.Zero, yn = T.Zero;

        while (r > T.Zero)
        {
            var q = a / b;
            xn = x0 - q * x1;
            yn = y0 - q * y1;

            x0 = x1;
            y0 = y1;
            x1 = xn;
            y1 = yn;
            a = b;
            b = r;
            r = a % b;
        }

        return (xn, yn);
    }

    /// <summary>
    /// Finds the number x where
    /// x ≡ p1.a mod p1.n
    /// and 
    /// x ≡ p2.a mod p2.n
    /// </summary>
    /// <returns>
    /// A tuple where (a: x, n: p1.n * p2.n)
    /// </returns>
    public static (T a, T n) ChineseRemainder<T>((T a, T n) p1, (T a, T n) p2)
        where T : INumber<T>
    {
        (T m1, T m2) = ExtEuclid(p1.n, p2.n);
        var n = p1.n * p2.n;
        var x = (p1.a * m2 * p2.n) % n;
        var y = (p2.a * m1 * p1.n) % n;
        return (Mod(x + y, n), n);
    }

    public static long AppendDigit(long num, char digit)
        => num * 10 + (digit - '0');

    public static TNumber Concat<TNumber>(TNumber a, TNumber b)
        where TNumber : INumber<TNumber>
    {
        var c = b;
        var ten = TNumber.CreateTruncating(10);
        while (c > TNumber.Zero)
        {
            a *= ten;
            c /= ten;
        }
        return a + b;
    }
    /// <summary>
    /// Creates a function to calculated f(x) from a set of 
    /// value pairs using newton polynomial interpolation.
    /// </summary>
    public static Func<double, double> InterpolateFromSamples(IEnumerable<(double x, double y)> samples)
    {
        var input = samples.OrderBy(n => n.x)
            .Select(p => (x1: p.x, x2: p.x, p.y))
            .ToArray();

        var b1 = input.First().y;
        var factors = input.Select(n => n.x1).SkipLast(1).Zip(
            input.Unfold(en => en.Pairwise((a,b) =>
            {
                var c = (b.y - a.y) / (b.x2 - a.x1);
                return (a.x1, b.x2, c);
            }).ToArray()).TakeWhile(f => f.Length > 0).Select(f => f.First().y)
        ).ToArray();

        return (x) =>
        {
            double sum = b1;
            double fact = 1;
            foreach(var (xn,bn) in factors)
            {
                fact *= x - xn;
                sum += bn * fact;
            }
            return sum;
        };
    }

    public static T Bisect<T>(T min, T max, Func<T, bool> predicate)
        where T : INumber<T>
    {
        var two = T.CreateChecked(2);
        while (min < max - T.One)
        {
            var mid = (max + min) / two;
            if (predicate(mid))
                min = mid;
            else
                max = mid;
        }
        return min;
    }

    
    public static void GaussElimination<T>(T[][] coeffs, T[] constants) where T : INumber<T>
    {
        if (constants.Length != coeffs.Length)
            throw new ArgumentException("Number of equations must match number of constants.");

        int rows = coeffs.Length;
        int cols = coeffs[0].Length;

        int pivotRow = 0;
        for (int col = 0; col < cols && pivotRow < rows; col++)
        {
            int pivot = -1;
            for (int row = pivotRow; row < rows; row++)
            {
                if (coeffs[row][col] != T.Zero)
                {
                    pivot = row;
                    break;
                }
            }

            if (pivot == -1) 
                continue;

            if (pivot != pivotRow)
            {
                (coeffs[pivot], coeffs[pivotRow]) = (coeffs[pivotRow], coeffs[pivot]);
                (constants[pivot], constants[pivotRow]) = (constants[pivotRow], constants[pivot]);
            }

            for (int row = pivotRow + 1; row < rows; row++)
            {
                if (coeffs[row][col] != T.Zero)
                {
                    T factor = coeffs[row][col] / coeffs[pivotRow][col];
                    for (int c = 0; c < cols; c++)
                        coeffs[row][c] -= factor * coeffs[pivotRow][c];
                    constants[row] -= factor * constants[pivotRow];
                }
            }

            pivotRow++;
        }

        for (int row = rows - 1; row >= 0; row--)
        {
            int pivotCol = -1;
            for (int col = 0; col < cols; col++)
            {
                if (coeffs[row][col] != T.Zero)
                {
                    pivotCol = col;
                    break;
                }
            }

            if (pivotCol == -1)
                continue;

            for (int r = 0; r < row; r++)
            {
                if (coeffs[r][pivotCol] != T.Zero)
                {
                    T factor = coeffs[r][pivotCol] / coeffs[row][pivotCol];
                    for (int c = 0; c < cols; c++)
                        coeffs[r][c] -= factor * coeffs[row][c];
                    constants[r] -= factor * constants[row];
                }
            }
        }
    }
}
