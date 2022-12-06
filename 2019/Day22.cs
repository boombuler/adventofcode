namespace AdventOfCode._2019;

using System.Numerics;

class Day22 : Solution<BigInteger>
{
    // a*x+b mod m
    record LCF(BigInteger A, BigInteger B, long M)
    {
        public static LCF Combine(LCF a, LCF b)
        {
            if (a.M != b.M)
                throw new InvalidOperationException();

            return new LCF(
                MathExt.Mod(a.A * b.A, a.M),
                MathExt.Mod((a.B * b.A) + b.B, a.M),
                a.M
            );
        }

        public LCF Repeat(BigInteger n)
        {
            var f = this;
            var result = new LCF(1, 0, M);
            while (n > 0)
            {
                if (n % 2 == 1)
                    result = Combine(result, f);
                n /= 2;
                f = Combine(f, f);
            }
            return result;
        }

        public long Apply(long x)
            => (long)MathExt.Mod((A * x) + B, M);

        public LCF Invert()
        {
            // ((x-B) / A) mod M
            //    --> a ^ - 1 = a ^ (m-2) mod m
            //    --> (a ^ -1) * x - (B * a ^ -1)
            var a1 = BigInteger.ModPow(A, M - 2, M);
            return new LCF(a1, -B * a1, M);
        }
    }

    private LCF BuildShuffleFunc(long deckSize, long rounds)
    {
        var result = Input.Lines().Select(line => line.Split(' '))
            .Select(l => int.TryParse(l.Last(), out int n) ? (l.First(), (int?)n) : (l.First(), null))
            .Select(l => l switch
            {
                ("deal", int n) => new LCF(n, 0, deckSize), // deal with increment N
                    ("cut", int n) => new LCF(1, -n, deckSize), // cut N
                    _ => new LCF(-1, -1, deckSize)              // deal into new stack
                }).Aggregate(LCF.Combine);
        return result.Repeat(rounds);
    }

    protected override BigInteger Part1()
        => BuildShuffleFunc(deckSize: 10007, rounds: 1).Apply(2019);

    protected override BigInteger Part2()
        => BuildShuffleFunc(deckSize: 119315717514047, rounds: 101741582076661).Invert().Apply(2020);
}
