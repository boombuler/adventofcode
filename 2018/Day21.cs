namespace AdventOfCode._2018;

using System;
using System.Collections.Generic;

class Day21 : Solution
{
    class Registers : ElfCompiler.Registers
    {
        private readonly int rIP;
        private readonly ElfCompiler.OpCodeLine[] fLines;
        private readonly Action<int> fEmitValue;
        private int? fCompReg = null;

        private ElfCompiler.OpCodeLine Current => fLines[base[rIP]];

        public override int this[int r]
        {
            get
            {
                if (r == 0 && Current.OpCode == "eqrr")
                {
                    if (!fCompReg.HasValue)
                        fCompReg = Current.A;
                    fEmitValue?.Invoke(base[fCompReg.Value]);
                }
                return base[r];
            }
            set => base[r] = value;
        }

        public Registers(string code, Action<int> emitValue)
        {
            (rIP, fLines) = ElfCompiler.Parse(code);
            fEmitValue = emitValue;
        }

    }

    protected override long? Part1()
    {
        var vm = ElfCompiler.CompileCode(Input);
        int result = 0;
        var regs = new Registers(Input, v =>
        {
            result = v;
            throw new ApplicationException();
        });

        try
        {
            vm(regs);
            return null;
        }
        catch (ApplicationException)
        {
            return result;
        }
    }

    protected override long? Part2()
    {
        var vm = ElfCompiler.CompileCode(Input);
        int result = 0;
        var seen = new HashSet<int>();

        var regs = new Registers(Input, v =>
        {
            if (seen.Add(v))
                result = v;
            else
                throw new ApplicationException();
        });

        try
        {
            vm(regs);
            return null;
        }
        catch (ApplicationException)
        {
            return result;
        }
    }
}
