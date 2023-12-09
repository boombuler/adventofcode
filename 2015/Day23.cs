namespace AdventOfCode._2015;

class Day23 : Solution
{
    record VM(long A, long B, int PC)
    {
        public static readonly VM Empty = new (0, 0, 0);

        public VM Run(string input)
        {
            var code = input.Lines().Select(Compile).ToList();
            var s = this;
            while (s.PC < code.Count)
            {
                s = code[s.PC](s);
                s = s with { PC = s.PC + 1 };
            }
            return s;
        }

        private static Func<VM, VM> Compile(string operation)
        {
            static Func<VM, VM> Jump(int offset, Func<VM, bool> condition = null) 
                => s => (condition?.Invoke(s) ?? true) ? s with { PC = s.PC - 1 + offset } : s;

            return operation.Replace(",", string.Empty).Split(' ') switch
            {
                ["hlf", "a"] => s => s with { A = s.A / 2 },
                ["hlf", "b"] => s => s with { B = s.B / 2 },
                ["tpl", "a"] => s => s with { A = s.A * 3 },
                ["tpl", "b"] => s => s with { B = s.B * 3 },
                ["inc", "a"] => s => s with { A = s.A + 1 },
                ["inc", "b"] => s => s with { B = s.B + 1 },
                ["jmp", var off] when int.TryParse(off, out int offset) => Jump(offset),
                ["jie", "a", var off] when int.TryParse(off, out int offset) => Jump(offset, s => s.A % 2 == 0),
                ["jie", "b", var off] when int.TryParse(off, out int offset) => Jump(offset, s => s.B % 2 == 0),
                ["jio", "a", var off] when int.TryParse(off, out int offset) => Jump(offset, s => s.A == 1),
                ["jio", "b", var off] when int.TryParse(off, out int offset) => Jump(offset, s => s.B == 1),
                _ => throw new NotImplementedException()
            };
        }
    }

    protected override long? Part1()
    {
        Assert(VM.Empty.Run(Sample()).A, 2);
        return VM.Empty.Run(Input).B;
    }

    protected override long? Part2() => new VM(1,0,0).Run(Input).B;
}
