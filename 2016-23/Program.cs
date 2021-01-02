using AdventHelper;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace _2016_23
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        public long RunCode(string inputFile, long a = 0)
        {
            var opcodes = ReadLines(inputFile).Select(s => s.Split(' ').ToImmutableArray()).ToImmutableArray();
            var toggles = Enumerable.Range(0, opcodes.Length).Select(_ => false).ToArray();
            long[] registers = new long[] { a, 0, 0, 0 };
            int PC = 0;

            while (PC < opcodes.Length)
            {
                var opcode = opcodes[PC];

                var operation = opcode[0];
                
                if (toggles[PC])
                {
                    switch (operation)
                    {
                        case "inc": operation = "dec"; break;
                        case "tgl":
                        case "dec": operation = "inc"; break;


                        case "jnz": operation = "cpy"; break;
                        case "cpy": operation = "jnz"; break;
                    }
                }


                int REG(int n) => opcode[n] switch
                {
                    "a" => 0,
                    "b" => 1,
                    "c" => 2,
                    "d" => 3,
                    _ => -1
                };

                long VAL(int n) => opcode[n] switch
                {
                    "a" => registers[0],
                    "b" => registers[1],
                    "c" => registers[2],
                    "d" => registers[3],
                    string other => long.Parse(other)
                };

                switch (operation)
                {
                    case "nop": break;
                    case "mul":
                        registers[REG(1)] = VAL(1) * VAL(2);
                        break;
                    case "cpy": // cpy x y copies x (either an integer or the value of a register) into register y.
                        var r = REG(2);
                        if (r >= 0)
                            registers[r] = VAL(1);
                        break;
                    case "inc": // inc x increases the value of register x by one.
                        registers[REG(1)] = registers[REG(1)] + 1;
                        break;
                    case "dec": // dec x decreases the value of register x by one.
                        registers[REG(1)] = registers[REG(1)] - 1;
                        break;
                    case "jnz": // jnz x y jumps to an instruction y away(positive means forward; negative means backward), but only if x is not zero.
                        if (VAL(1) != 0)
                            PC += (int)(VAL(2)-1);
                        break;
                    case "tgl":
                        var line = VAL(1) + PC;
                        if (line > 0 && line < toggles.Length)
                            toggles[line] = !toggles[line];
                        break;
                }
                PC++;
            }

            return registers[0];
        }



        protected override long? Part1()
        {
            Assert(RunCode("Sample", 0), 3);
            return RunCode("Input", 7);
        }

        protected override long? Part2() => RunCode("InputOptimized", 12);
    }
}
