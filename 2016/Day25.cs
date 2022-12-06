namespace AdventOfCode._2016;

class Day25 : Solution
{
    enum OpCode { cpy, inc, dec, jnz, @out };

    class VM : AsmVM<OpCode>
    {
        public VM(string code)
            : base(code)
        { }

        public IEnumerable<int> RunCode(int a)
        {
            this["a"] = a;

            var prevStates = new HashSet<(long a, long b, long c, long d, int pc)>();

            while (!Finished)
            {
                if (!prevStates.Add((this["a"], this["b"], this["c"], this["d"], PC)))
                    yield break; // since this state was already reached, everything after this would repeat the pattern.

                switch (OpCode)
                {
                    case OpCode.cpy: Y = X; break;
                    case OpCode.inc: X++; break;
                    case OpCode.dec: X--; break;
                    case OpCode.jnz: // jnz x y jumps to an instruction y away(positive means forward; negative means backward), but only if x is not zero.
                        if (X != 0)
                            PC += (int)(Y - 1);
                        break;
                    case OpCode.@out:
                        yield return (int)X;
                        break;
                }
                PC++;
            }
        }
    }

    protected override long? Part1()
    {
        for (int i = 0; i < int.MaxValue; i++)
        {
            int last = -1;
            bool ok = true;
            int first = -1;
            foreach (var cur in new VM(Input).RunCode(i))
            {
                if (first == -1)
                    first = cur;
                else if ((last == 0 && cur != 1) || (last == 1 && cur != 0))
                {
                    ok = false;
                    break;
                }

                last = cur;
            }
            if (ok && last != first)
                return i;
        }
        return null;
    }

}
