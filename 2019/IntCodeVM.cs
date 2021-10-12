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
        private const long OC_ADJRBA = 9; // Adjust relative base offset.

        private const long PM_POS = 0;
        private const long PM_IMM = 1;
        private const long PM_REL = 2;

        private long[] fCode;
        private Dictionary<long, long> fData;
        private long fPC;
        private long fRelativeBaseOffset;
        public IntCodeVM(string code)
        {
            fCode = code.Split(',').Select(long.Parse).ToArray();
            Reset();
        }

        public void Reset()
        {
            fPC = 0;
            fRelativeBaseOffset = 0;
            fData = fCode.Select((v, i) => (v, i)).ToDictionary(x => (long)x.i, x => x.v);
        }

        public long this[long idx]
        {
            get => idx < 0 ? throw new InvalidOperationException() : (fData.TryGetValue(idx, out var res) ? res : 0);
            set => fData[idx] = value;
        }

        private long Param(ref long opcode)
        {
            var m = opcode % 10;
            opcode = opcode / 10;
            var value = this[fPC++];
            switch(m)
            {
                case PM_POS: return this[value];
                case PM_IMM: return value;
                case PM_REL: return this[value + fRelativeBaseOffset];
                default: throw new NotImplementedException();
            }
        }

        private void SetParam(ref long opcode, long value)
        {
            var m = opcode % 10;
            opcode = opcode / 10;

            var addr = this[fPC++];
            switch (m)
            {
                case PM_IMM: // Setting to IMM behaves like POS mode.
                case PM_POS: this[addr] = value; break;
                case PM_REL: this[addr + fRelativeBaseOffset] = value; break;
                default: throw new NotImplementedException();
            }
        }

        private void BinaryOp(long p, Func<long, long, long> op)
        {
            var (a, b) = (Param(ref p), Param(ref p));
            SetParam(ref p, op(a, b));
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
                        SetParam(ref p, input.Current);
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
                case OC_ADJRBA:
                        fRelativeBaseOffset += Param(ref p); break;
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
