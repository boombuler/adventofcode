namespace AdventOfCode._2016;

class Day12 : Solution
{
    enum OpCode
    {
        cpy, inc, dec, jnz
    }

    class VM : AsmVM<OpCode>
    {
        public VM(string code)
            : base(code)
        { }

        public long Run(long cInit = 0)
        {
            this["c"] = cInit;

            while (!Finished)
            {
                switch (OpCode)
                {
                    case OpCode.cpy: Y = X; break; // cpy x y copies x (either an integer or the value of a register) into register y.
                    case OpCode.inc: X++; break; // inc x increases the value of register x by one.
                    case OpCode.dec: X--; break;// dec x decreases the value of register x by one.
                    case OpCode.jnz: // jnz x y jumps to an instruction y away(positive means forward; negative means backward), but only if x is not zero.
                        if (X != 0)
                            PC += (int)(Y - 1);
                        break;
                }
                PC++;
            }

            return this["a"];
        }
    }

    protected override long? Part1()
    {
        Assert(new VM(Sample()).Run(), 42);
        return new VM(Input).Run();
    }

    protected override long? Part2() => new VM(Input).Run(1);
}
