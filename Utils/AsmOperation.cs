using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    class AsmOperation<TOpCode> where TOpCode: struct
    {
        public TOpCode Kind { get; }
        public string X { get; }
        public string Y { get; }

        public AsmOperation(string line)
        {
            var parts = line.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            Kind = Enum.Parse<TOpCode>(parts[0]);
            X = (parts.Length > 1) ? parts[1] : null;
            Y = (parts.Length > 2) ? parts[2] : null;
        }
    }
}
