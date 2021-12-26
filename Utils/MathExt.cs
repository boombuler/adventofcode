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

    public static BigInteger Mod(BigInteger a, BigInteger m) => ((a % m) + m) % m;

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
}
