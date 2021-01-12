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

        class VM : AsmVM<OpCode>
        {
            private readonly bool MultiThreading;
            private readonly Queue<long> InputQueue = new Queue<long>();

            public VM(string code, long? PID = null)
                : base(code)
            {
                MultiThreading = PID.HasValue;
                
                this["p"] = PID ?? 0;
            }

            public void Enqueue(long value) => InputQueue.Enqueue(value);
            public long? Run()
            {
                while (true)
                {
                    switch (OpCode)
                    {
                        case OpCode.snd:
                            var x = X;
                            PC++;
                            return x; 
                        case OpCode.set: X = Y; break;
                        case OpCode.add: X += Y; break;
                        case OpCode.mul: X *= Y; break;
                        case OpCode.mod: X %= Y; break;
                        case OpCode.rcv:
                            if (MultiThreading || X != 0)
                            {
                                if (InputQueue.TryDequeue(out long v))
                                    X = v;
                                else
                                    return null;
                            }
                            break;
                        case OpCode.jgz:
                            if (X > 0)
                                PC += (int)Y - 1;
                            break;
                    }
                    PC++;
                }
            }

        }

        private long RecoverSounds(string code)
        {
            var vm = new VM(code);
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
            var vm0 = new VM(code, 0);
            var vm1 = new VM(code, 1);
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
