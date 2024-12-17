namespace AdventOfCode._2024;

using static Parser;

class Day17 : Solution<string, long>
{
    private static readonly Parser<(long A, long B, long C, int[] Programm)> InputParser =
        from a in "Register A: " + Long + "\n"
        from b in "Register B: " + Long + "\n"
        from c in "Register C: " + Long + "\n\n"
        from nums in "Program: " + Int.List(',')
        select (a, b, c, nums);

    private IEnumerable<int> Execute(long a, long b, long c, int[] programm)
    {
        int ip = 0;
        while (ip < programm.Length)
        {
            long Combo() => (programm[ip + 1] & 7) switch
            {
                4 => a,
                5 => b,
                6 => c,
                int x => x
            };
            int Literal() => programm[ip + 1] & 7;

            switch (programm[ip] & 7)
            {
                case 0: a = a >> (int)Combo(); break;
                case 1: b = b ^ Literal(); break;
                case 2: b = Combo() % 8; break;
                case 3: ip = (a != 0) ? Literal() - 2 : ip; break;
                case 4: b = b ^ c; break;
                case 5: yield return (int)(Combo() % 8); break;
                case 6:  b = a >> (int)Combo(); break;
                case 7:  c = a >> (int)Combo(); break;
            }
            ip += 2;
        }
    }

    protected override string Part1()
    {
        string Solve(string input)
        {
            var (a, b, c, programm) = InputParser.MustParse(input);
            var result = Execute(a, b, c, programm);
            return string.Join(",", result);
        }

        Assert(Solve(Sample()), "4,6,3,5,6,3,5,2,1,0");
        return Solve(Input);
    }

    protected override long Part2()
    {
        var (_, b, c, programm) = InputParser.MustParse(Input);
        
        var open = new Stack<(long a, int index)>();
        open.Push((0, programm.Length - 1));
        long min = long.MaxValue;
        while(open.TryPop(out var cur))
        {
            for (int i = 0; i < 8; i++)
            {
                var a = (cur.a << 3) ^ i;
                if (Execute(a, b, c, programm).First() != programm[cur.index])
                    continue;
                
                if (cur.index == 0)
                    min = Math.Min(a, min);
                else
                    open.Push((a, cur.index - 1));
            }
        }

        return min;
    }
}
