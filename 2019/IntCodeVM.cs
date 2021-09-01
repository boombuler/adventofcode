using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2019
{
    class IntCodeVM
    {
        private const long OC_HALT = 99;
        private const long OC_ADD = 1;
        private const long OC_MUL = 2;
        private const long OC_IN = 3;
        private const long OC_OUT = 4;
        private const long OC_JNZ = 5;
        private const long OC_JZ = 6;
        private const long OC_LT = 7;
        private const long OC_EQ = 8;

        private const long PM_POS = 0;
        private const long PM_IMM = 1;

        private long[] fCode;
        private Dictionary<long, long> fData;
        private long fPC;
        public IntCodeVM(string code)
        {
            fCode = code.Split(',').Select(long.Parse).ToArray();
            Reset();
        }

        public void Reset()
        {
            fPC = 0;
            fData = fCode.Select((v, i) => (v, i)).ToDictionary(x => (long)x.i, x => x.v);
        }

        public long this[long idx]
        {
            get => fData.TryGetValue(idx, out var res) ? res : 0;
            set => fData[idx] = value;
        }

        private long Param(ref long opcode, long? forcedMode = null)
        {
            var m = opcode % 10;
            opcode = opcode / 10;

            m = forcedMode ?? m;

            var value = this[fPC++];
            switch(m)
            {
                case PM_POS: return this[value];
                case PM_IMM: return value;
                default: throw new NotImplementedException();
            }
        }

        private void BinaryOp(long p, Func<long, long, long> op)
        {
            var (a, b, c) = (Param(ref p), Param(ref p), Param(ref p, PM_IMM));
            this[c] = op(a, b);
        }

        private (long? Value, bool Continue) Step(IEnumerator<long> input)
        {
            var opCode = this[fPC++];
            var p = opCode / 100;

            switch (opCode % 100)
            {
                case OC_HALT: 
                    return (null, false);
                case OC_ADD: BinaryOp(p, (a, b) => a + b); break;
                case OC_MUL: BinaryOp(p, (a, b) => a * b); break;
                case OC_LT: BinaryOp(p, (a, b) => a < b ? 1 : 0); break;
                case OC_EQ: BinaryOp(p, (a, b) => a == b ? 1 : 0); break;
                case OC_OUT: return (Param(ref p), true);
                case OC_IN:
                    {
                        if (!input.MoveNext())
                            throw new InvalidOperationException("missing input");
                        var r = Param(ref p, PM_IMM);
                        this[r] = input.Current;
                    } break;
                case OC_JNZ:
                    {
                        var (test, target) = (Param(ref p), Param(ref p));
                        if (test != 0)
                            fPC = target;
                    } break;
                case OC_JZ:
                    {
                        var (test, target) = (Param(ref p), Param(ref p));
                        if (test == 0)
                            fPC = target;
                    }
                    break;
                

                default:
                    throw new InvalidOperationException("Unsupported OpCode"); 
            }
            return (null, true);
        }

        public IEnumerable<long> Run(IEnumerable<long> input = null)
        {
            input ??= Enumerable.Empty<long>();
            var inputEnumerator = input.GetEnumerator();
            bool cont;
            do
            {
                long? v;
                (v, cont) = Step(inputEnumerator);
                if (v.HasValue)
                    yield return v.Value;
            }
            while (cont);
        }

    }
}
