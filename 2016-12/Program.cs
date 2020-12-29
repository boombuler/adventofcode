using AdventHelper;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace _2016_12
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        public long RunCode(string inputFile, long cInit = 0)
        {
            var opcodes = ReadLines(inputFile).Select(s => s.Split(' ').ToImmutableArray()).ToImmutableArray();
            long[] registers = new long[] { 0, 0, cInit, 0 };
            int PC = 0;

            while (PC < opcodes.Length)
            {
                var opcode = opcodes[PC++];

                int REG(int n) => opcode[n] switch
                {
                    "a" => 0,
                    "b" => 1,
                    "c" => 2,
                    _ => 3
                };

                long VAL(int n) => opcode[n] switch
                {
                    "a" => registers[0],
                    "b" => registers[1],
                    "c" => registers[2],
                    "d" => registers[3],
                    string other => long.Parse(other)
                };

                switch (opcode[0])
                {
                    case "cpy": // cpy x y copies x (either an integer or the value of a register) into register y.
                        registers[REG(2)] = VAL(1); break;
                    case "inc": // inc x increases the value of register x by one.
                        registers[REG(1)] = registers[REG(1)] + 1; break;
                    case "dec": // dec x decreases the value of register x by one.
                        registers[REG(1)] = registers[REG(1)] - 1; break;
                    case "jnz": // jnz x y jumps to an instruction y away(positive means forward; negative means backward), but only if x is not zero.
                        if (VAL(1) != 0)
                            PC += (int)(VAL(2) - 1);
                        break;
                }
            }

            return registers[0];
        }


        protected override long? Part1()
        {
            Assert(RunCode("Sample"), 42);
            return RunCode("Input");
        }

        protected override long? Part2() => RunCode("Input", 1);
    }
}
