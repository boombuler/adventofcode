using AdventHelper;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2015_23
{
    class Program : ProgramBase
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

        private static readonly Func<string, Operation> ParseOperation = new Regex(@"(?<Op>\w{3}) (?<Reg>[ab])?(, )?(?<Offset>[\+\-]?\d+)?", RegexOptions.Compiled).ToFactory<Operation>();
        class Operation
        {
            public OpCode Op { get; set; }
            public Register Reg { get; set; }
            public int Offset { get; set; }
        }

        static void Main(string[] args) => new Program().Run();

        private long RunProgram(string inputFile, Register reg, ulong initialA = 0)
        {
            var program = ReadLines(inputFile).Select(ParseOperation).ToList();

            ulong A = initialA, B = 0;
            int PC = 0;

            while (PC < program.Count)
            {
                var o = program[PC++];
                switch(o.Op)
                {
                    case OpCode.hlf:
                        if (o.Reg == Register.a)
                            A = A / 2;
                        else
                            B = B / 2;
                        break;
                    case OpCode.tpl:
                        if (o.Reg == Register.a)
                            A = A * 3;
                        else
                            B = B * 3;
                        break;
                    case OpCode.inc:
                        if (o.Reg == Register.a)
                            A++;
                        else
                            B++;
                        break;
                    case OpCode.jmp:
                        PC = (PC - 1) + o.Offset; 
                        break;
                    case OpCode.jie:
                        if (((o.Reg == Register.a) ? A : B) % 2 == 0)
                            PC = (PC - 1) + o.Offset;
                        break;
                    case OpCode.jio:
                        if (((o.Reg == Register.a) ? A : B) == 1)
                            PC = (PC - 1) + o.Offset;
                        break;
                }
            }
            return (long)((reg == Register.a) ? A : B);
        }

        protected override long? Part1()
        {
            Assert(RunProgram("Sample", Register.a), 2);
            return RunProgram("Input", Register.b);
        }

        protected override long? Part2() => RunProgram("Input", Register.b, 1);
    }
}
