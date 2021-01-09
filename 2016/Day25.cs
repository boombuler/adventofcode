using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AdventOfCode._2016
{
    class Day25 : Solution
    {
        public IEnumerable<int> RunCode(string input, int a)
        {
            var opcodes = input.Lines().Select(s => s.Split(' ').ToImmutableArray()).ToImmutableArray();
            int[] registers = new int[] { a, 0, 0, 0 };
            int PC = 0;

            var prevStates = new HashSet<(int a, int b, int c, int d, int pc)>();

            while (PC < opcodes.Length)
            {
                if (!prevStates.Add((registers[0], registers[1], registers[2], registers[3], PC)))
                    yield break; // since this state was already reached, everything after this would repeat the pattern.

                var opcode = opcodes[PC];

                var operation = opcode[0];

                int REG(int n) => opcode[n] switch
                {
                    "a" => 0,
                    "b" => 1,
                    "c" => 2,
                    "d" => 3,
                    _ => -1
                };

                int VAL(int n) => opcode[n] switch
                {
                    "a" => registers[0],
                    "b" => registers[1],
                    "c" => registers[2],
                    "d" => registers[3],
                    string other => int.Parse(other)
                };

                switch (operation)
                {
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
                            PC += (int)(VAL(2) - 1);
                        break;
                    case "out":
                        yield return VAL(1);
                        break;
                }
                PC++;
            }
        }

        protected override long? Part1()
        {
            for (int i = 0; i < int.MaxValue; i++)
            {
                int last = -1;
                bool ok = true;
                int first = -1;
                foreach(var cur in RunCode(Input, i))
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
}
