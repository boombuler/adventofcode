namespace AdventOfCode._2021;

class Day24 : Solution
{
    private static Func<long, IEnumerable<(long Z, int Digit)>> StepGenerator(int a, int b)
    {
        if (a < 0)
            return z =>
            {
                var w = (int)((z % 26) + a);
                if (w is <= 0 or > 9)
                    return Enumerable.Empty<(long Z, int Digit)>();
                z /= 26;
                return Enumerable.Repeat((z, w), 1);
            };
        else
            return z =>
            {
                z = (z * 26) + b;
                return Enumerable.Range(1, 9).Select(w => (z + w, w));
            };
    }
    record State(long Digits, int DigitIdx, long Z);

    public long GetModelNumber(bool max)
    {
        var digitGenerators = Input.Lines()
            .Select(line => line.Split(' ').Last()).Chunk(18)
            .Select(d => StepGenerator(int.Parse(d.ElementAt(5)), int.Parse(d.ElementAt(15))))
            .ToList();
        if (!max)
            digitGenerators = digitGenerators.Select(f => f.Combine(Enumerable.Reverse)).ToList();

        var open = new Stack<State>();
        open.Push(new State(0, 0, 0));

        while (open.TryPop(out var s))
        {
            var nextDigit = s.DigitIdx + 1;
            foreach (var (zz, d) in digitGenerators[s.DigitIdx](s.Z))
            {
                var digits = (s.Digits * 10) + d;
                if (nextDigit == digitGenerators.Count)
                {
                    if (zz == 0)
                        return digits;
                }
                else
                    open.Push(new State(digits, nextDigit, zz));
            }
        }
        return 0;
    }

    protected override long? Part1() => GetModelNumber(true);

    protected override long? Part2() => GetModelNumber(false);
}
