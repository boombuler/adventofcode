using AdventOfCode.Utils;
using System.Linq;

namespace AdventOfCode._2017
{
    class Day23 : Solution
    {
        enum OpCode
        {
            set, // X Y sets register X to the value of Y.
            sub, // X Y decreases register X by the value of Y.
            mul, // X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
            jnz, // X Y jumps with an offset of the value of Y, but only if the value of X is not zero. (An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)
        }

        class VM : AsmVM<OpCode>
        {
            public VM(string code) 
                : base(code)
            {
            }

            public long CountMul()
            {
                long result = 0;
                while (!Finished)
                {
                    switch (OpCode)
                    {
                        case OpCode.set: X  = Y; break;
                        case OpCode.sub: X -= Y; break;
                        case OpCode.mul: X *= Y; result++; break;
                        case OpCode.jnz:
                            if (X != 0)
                                PC += (int)Y - 1;
                            break;
                    }
                    PC++;
                }
                return result;
            }
        }

        protected override long? Part1() => new VM(Input).CountMul();

        protected override long? Part2()
        {
            long b = long.Parse(Input.Lines().Select(l => new AsmOperation<OpCode>(l)).First().Y);

            long result = 0;
            b = (b * 100) + 100000;
            long upperBound = b + 17000 + 17;
           
            for (; b < upperBound; b+=17)
            {
                // Inc result if b is not a prime.
                for (long d = 2; d * d <= b; d++)
                {
                    if ((b % d) == 0)
                    {
                        result++;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
