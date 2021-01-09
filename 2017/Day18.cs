using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2017
{
    class Day18 : Solution
    {
        enum OpCode
        {
            snd, // snd X plays a sound with a frequency equal to the value of X.
            set, // set X Y sets register X to the value of Y.
            add, // add X Y increases register X by the value of Y.
            mul, // mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
            mod, // mod X Y sets register X to the remainder of dividing the value contained in register X by the value of Y (that is, it sets X to the result of X modulo Y).
            rcv, // rcv X recovers the frequency of the last sound played, but only when the value of X is not zero. (If it is zero, the command does nothing.)
            jgz, // jgz X Y jumps with an offset of the value of Y, but only if the value of X is greater than zero. (An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)
        }

        class Operation
        {
            public OpCode Kind {get;}
            public string X { get; }
            public string Y { get; }

            public Operation(string line)
            {
                var parts =  line.Split(' ');
                Kind = Enum.Parse<OpCode>(parts[0]);
                X = parts[1];
                Y = (parts.Length > 2) ? parts[2] : null;
            }
        }

        class VM
        {
            private int PC = 0;
            private readonly long[] Registers = new long[1 + ('z' - 'a')];
            private readonly bool MultiThreading;
            private readonly ImmutableList<Operation> Code;
            private readonly Queue<long> InputQueue = new Queue<long>();

            public VM(ImmutableList<Operation> code, long? PID = null)
            {
                Code = code;
                MultiThreading = PID.HasValue;
                
                this["p"] = PID ?? 0;
            }

            public void Enqueue(long value) => InputQueue.Enqueue(value);
            public long? Run()
            {
                while (true)
                {
                    var op = Code[PC];

                    switch (op.Kind)
                    {
                        case OpCode.snd:
                            PC++;
                            return this[op.X]; 
                        case OpCode.set: this[op.X] = this[op.Y]; break;
                        case OpCode.add: this[op.X] += this[op.Y]; break;
                        case OpCode.mul: this[op.X] *= this[op.Y]; break;
                        case OpCode.mod: this[op.X] %= this[op.Y]; break;
                        case OpCode.rcv:
                            if (MultiThreading || this[op.X] != 0)
                            {
                                if (InputQueue.TryDequeue(out long v))
                                    this[op.X] = v;
                                else
                                    return null;
                            }
                            break;
                        case OpCode.jgz:
                            if (this[op.X] > 0)
                                PC += (int)this[op.Y] - 1;
                            break;
                    }
                    PC++;
                }
            }

            public long this[string arg]
            {
                get
                {
                    if (long.TryParse(arg, out long val))
                        return val;
                    return Registers[arg[0] - 'a'];
                }
                set
                {
                    Registers[arg[0] - 'a'] = value;
                }
            }
        }

        private long RecoverSounds(string code)
        {
            var operations = code.Lines().Select(l => new Operation(l)).ToImmutableList();
            var vm = new VM(operations);
            var result = 0L;
            while (true)
            {
                var next = vm.Run();
                if (next.HasValue)
                    result = next.Value;
                else
                    return result;
            }
        }

        private long PlayDuet(string code)
        {
            var operations = code.Lines().Select(l => new Operation(l)).ToImmutableList();
            var vm0 = new VM(operations, 0);
            var vm1 = new VM(operations, 1);
            long result = 0;
            while (true)
            {
                (var val0, var val1) = (vm0.Run(), vm1.Run());
                if (!val0.HasValue && !val1.HasValue)
                    return result;
                else
                {
                    if (val0.HasValue)
                        vm1.Enqueue(val0.Value);
                    if (val1.HasValue)
                    {
                        result++;
                        vm0.Enqueue(val1.Value);
                    }
                }
            }
        }

        protected override long? Part1()
        {
            Assert(RecoverSounds(Sample()), 4);
            return RecoverSounds(Input);
        }

        protected override long? Part2()
        {
            Assert(PlayDuet(Sample("Duet")), 3);
            return PlayDuet(Input);
        }
    }
}
