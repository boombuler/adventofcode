using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Utils
{
    record AsmOperation<TOpCode>(TOpCode Kind, string X, string Y) where TOpCode: struct
    {
        public static AsmOperation<TOpCode> Parse(string line)
        {
            var parts = line.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            return new AsmOperation<TOpCode>(
                Enum.Parse<TOpCode>(parts[0]),
                (parts.Length > 1) ? parts[1] : null,
                (parts.Length > 2) ? parts[2] : null);
        }
    }
}
