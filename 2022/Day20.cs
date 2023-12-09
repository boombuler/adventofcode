namespace AdventOfCode._2022;

class Day20 : Solution
{
    class Number(string n, long decryptionKey)
    {
        public long Value { get; } = long.Parse(n) * decryptionKey;
    }

    private static long DecryptFile(string input, long decryptionKey, int rounds)
    {
        var numbers = input.Lines().Select(s => new Number(s, decryptionKey)).ToArray();
        var steps = numbers.ToArray();

        for (int round = 0; round < rounds; round++)
        {
            foreach (var step in steps)
            {
                var i = Array.IndexOf(numbers, step);
                var swaps = step.Value % (numbers.Length - 1);
                var dir = Math.Sign(swaps);
                for (int n = 0; n != swaps; n += dir)
                {
                    var j = (i + dir + numbers.Length) % numbers.Length;
                    (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
                    i = j;
                }
            }
        }
        int zero = Array.FindIndex(numbers, n => n.Value == 0);
        long Indexed(int i) => numbers[(i + zero) % numbers.Length].Value;
        return Indexed(1000) + Indexed(2000) + Indexed(3000);
    }

    protected override long? Part1()
    {
        static long Solve(string input) => DecryptFile(input, 1, 1);
        Assert(Solve(Sample()), 3);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input) => DecryptFile(input, 811589153, 10);
        Assert(Solve(Sample()), 1623178306);
        return Solve(Input);
    }
}
