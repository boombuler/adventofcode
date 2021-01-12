using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    abstract class AsmVM<TOpCode> 
        where TOpCode : struct
    {
        private readonly List<AsmOperation<TOpCode>> fOpCodes;
        private readonly Dictionary<string, long> fRegisters = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        private AsmOperation<TOpCode> CurOpCode => !Finished ? fOpCodes[PC] : null;
        protected int PC { get; set; } = 0;
        protected bool Finished => PC < 0 || PC >= fOpCodes.Count;
        protected long X
        {
            get
            {
                if (long.TryParse(CurOpCode.X, out long val))
                    return val;
                return this[CurOpCode.X];
            }
            set
            {
                this[CurOpCode.X] = value;
            }
        }

        protected long Y
        {
            get
            {
                if (long.TryParse(CurOpCode.Y, out long val))
                    return val;
                return this[CurOpCode.Y];
            }
            set => this[CurOpCode.Y] = value;
        }

        protected TOpCode OpCode => CurOpCode.Kind;

        protected long this[string register]
        {
            get => fRegisters.GetValueOrDefault(register);
            set => fRegisters[register] = value;
        }

        public AsmVM(string code)
        {
            fOpCodes = code.Lines().Select(o => new AsmOperation<TOpCode>(o)).ToList();
        }
    }
}
