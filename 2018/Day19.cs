using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode._2018
{
    class Day19 : Solution
    {   
        private int RunCompiled(string code, int initialR0 = 0)
        {
            var vm = ElfCompiler.CompileCode(code);
            var regs = new ElfCompiler.Registers();
            regs[0] = initialR0;
            vm(regs);
            return regs[0];
        }

        protected override long? Part1()
        {
            Assert(RunCompiled(Sample()), 6);
            return RunCompiled(Input);
        }

        protected override long? Part2() => RunCompiled(Input, 1);
    }
}
