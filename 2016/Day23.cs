namespace AdventOfCode._2016;

using System.Collections.Generic;
using AdventOfCode.Utils;

class Day23 : Solution
{
    enum OpCode
    {
        tgl, inc, dec, jnz, cpy,

        nop, mul,
    }

    class VM : AsmVM<OpCode>
    {
        public VM(string input)
            : base(OptimizeMultiplication(input))
        {
        }

        private static string OptimizeMultiplication(string input)
        {
            const string PATTERN = @"cpy a d
cpy 0 a
cpy b c
inc a
dec c
jnz c -2
dec d
jnz d -5";
            const string REPLACEMENT = @"mul a b
nop
nop
nop
nop
nop
nop
nop";
            return input.Replace(PATTERN.Replace("\r", string.Empty), REPLACEMENT);
        }

        public long RunCode(long a = 0)
        {
            var toggles = new HashSet<int>();
            PC = 0;
            this["a"] = a;

            while (!Finished)
            {
                OpCode operation = OpCode;
                if (toggles.Contains(PC))
                {
                    switch (operation)
                    {
                        case OpCode.inc: operation = OpCode.dec; break;
                        case OpCode.tgl:
                        case OpCode.dec: operation = OpCode.inc; break;
                        case OpCode.jnz: operation = OpCode.cpy; break;
                        case OpCode.cpy: operation = OpCode.jnz; break;
                    }
                }

                switch (operation)
                {
                    case OpCode.nop: break;
                    case OpCode.mul: X *= Y; break;
                    case OpCode.cpy: Y = X; break;
                    case OpCode.inc: X++; break;
                    case OpCode.dec: X--; break;
                    case OpCode.jnz: // jnz x y jumps to an instruction y away(positive means forward; negative means backward), but only if x is not zero.
                        if (X != 0)
                            PC += (int)(Y - 1);
                        break;
                    case OpCode.tgl:
                        var line = (int)X + PC;
                        if (!toggles.Add(line))
                            toggles.Remove(line);
                        break;
                }
                PC++;
            }

            return this["a"];
        }
    }

    protected override long? Part1()
    {
        Assert(new VM(Sample()).RunCode(0), 3);
        return new VM(Input).RunCode(7);
    }

    protected override long? Part2() => new VM(Input).RunCode(12);
}
