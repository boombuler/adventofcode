using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AdventHelper
{
    public class ChineseRemainder
    {
        private static (BigInteger, BigInteger) extEuclid(BigInteger a, BigInteger b)
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
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static (BigInteger a, BigInteger n) Solve((BigInteger a, BigInteger n) p1, (BigInteger a, BigInteger n) p2)
        {
            (BigInteger m1, BigInteger m2) = extEuclid(p1.n, p2.n);
            var n = p1.n * p2.n;
            var x = (p1.a * m2 * p2.n) % n;
            var y = (p2.a * m1 * p1.n) % n;
            var a = (x + y) % n;
            if (a < 0)
                a += n;
            return (a, n);
        }

    }
}
