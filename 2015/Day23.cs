namespace AdventOfCode._2015;

class Day23 : Solution
{
    enum OpCode
    {
        hlf,
        tpl,
        inc,
        jmp,
        jie,
        jio
    }
    enum Register { a, b }

    class VM : AsmVM<OpCode>
    {
        public VM(string code)
            : base(code)
        {
        }

        private long this[Register r]
        {
            get => base[r.ToString()];
            set => base[r.ToString()] = value;
        }

        public long RunProgram(Register reg, long initialA = 0)
        {
            this[Register.a] = initialA;
            while (!Finished)
            {
                switch (OpCode)
                {
                    case OpCode.hlf: X /= 2; break;
                    case OpCode.tpl: X *= 3; break;
                    case OpCode.inc: X += 1; break;
                    case OpCode.jmp:
                        PC = (PC - 1) + (int)X;
                        break;
                    case OpCode.jie:
                        if (X % 2 == 0)
                            PC = (PC - 1) + (int)Y;
                        break;
                    case OpCode.jio:
                        if (X == 1)
                            PC = (PC - 1) + (int)Y;
                        break;
                }
                PC++;
            }
            return this[reg];
        }
    }

    protected override long? Part1()
    {
        Assert(new VM(Sample()).RunProgram(Register.a), 2);
        return new VM(Input).RunProgram(Register.b);
    }

    protected override long? Part2() => new VM(Input).RunProgram(Register.b, 1);
}
