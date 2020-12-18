using AdventHelper;
using System;
using System.Linq;
using System.Numerics;

namespace _2020_13
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private long GetNextDepatureId(string inputFile)
        {
            var lines = ReadLines(inputFile);
            var minDepatureTime = long.Parse(lines.First());
            var depaturePlan = lines.Last().Split(',').Where(id => id != "x")
                .Select(long.Parse)
                .Select(id => new { id, depatureTime = minDepatureTime + id - (minDepatureTime % id) });
            var bus = depaturePlan.OrderBy(p => p.depatureTime).First();
            return bus.id * (bus.depatureTime - minDepatureTime);
        }

        public (BigInteger, BigInteger) extEuclid(BigInteger a, BigInteger b)
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

        private (BigInteger a, BigInteger n) ChineseRemainder((BigInteger a, BigInteger n) p1, (BigInteger a, BigInteger n) p2)
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

        private long GetContestWinningMinute(string inputFile)
        {
            var busses = ReadLines(inputFile).Last().Split(',')
                .Select((s, i) => new { ID = s, idx = i })
                .Where(x => x.ID != "x")
                .Select(x => new { ID = long.Parse(x.ID), Idx = (long)x.idx });

            var result = busses.Select(b => (a: (BigInteger)((b.ID - b.Idx) % b.ID), n: (BigInteger)b.ID)).Aggregate(ChineseRemainder);
            if (result.a < 0)
                return (long)(result.a + result.n);
            return (long)result.a;
        }
        protected override long? Part1()
        {
            Assert(GetNextDepatureId("Sample.txt"), 295);
            return GetNextDepatureId("Input.txt");
        }

        protected override long? Part2()
        {
            Assert(ChineseRemainder((2, 8), (4, 5)).a, 34);
            Assert(GetContestWinningMinute("Sample.txt"), 1068781);
            return GetContestWinningMinute("Input.txt");
        }
    }
}
