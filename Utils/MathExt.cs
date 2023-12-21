namespace AdventOfCode.Utils;

using System.Numerics;

class MathExt
{
    public static long GCD(long a, long b)
    {
        while (b != 0)
            (a, b) = (b, a % b);
        return a;
    }
    public static long LCM(long a, long b)
    {
        var gcd = GCD(a, b);
        return (a / gcd) * b;
    }

    public static T Mod<T>(T a, T m)
        where T : INumber<T>
        => ((a % m) + m) % m;

    /// <summary>
    /// Solves `ax + by = GCD(a, b)`
    /// </summary>
    public static (BigInteger x, BigInteger y) ExtEuclid(BigInteger a, BigInteger b)
    {
        BigInteger x0 = 1;
        BigInteger y0 = 0;
        BigInteger x1 = 0;
        BigInteger y1 = 1;
        BigInteger r = a % b;
        BigInteger xn = 0, yn = 0;

        while (r > 0)
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
    public static (BigInteger a, BigInteger n) ChineseRemainder((BigInteger a, BigInteger n) p1, (BigInteger a, BigInteger n) p2)
    {
        (BigInteger m1, BigInteger m2) = ExtEuclid(p1.n, p2.n);
        var n = p1.n * p2.n;
        var x = (p1.a * m2 * p2.n) % n;
        var y = (p2.a * m1 * p1.n) % n;
        return (Mod(x + y, n), n);
    }

    public static long AppendDigit(long num, char digit)
        => num * 10 + (digit - '0');

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
            input.Unfold(en => en.SlidingWindow(2).Select(wnd =>
            {
                var c = (wnd[1].y - wnd[0].y) / (wnd[1].x2 - wnd[0].x1);
                return (wnd[0].x1, wnd[1].x2, c);
            }).ToArray()).TakeWhile(f => f.Length > 0).Select(f => f.First().y)
        ).ToArray();

        return (x) =>
        {
            double sum = b1;
            double fact = 1;
            foreach(var f in factors)
            {
                fact *= x - f.First;
                sum += f.Second * fact;
            }
            return sum;
        };
    }
}
