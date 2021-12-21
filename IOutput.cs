using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    interface IOutput
    {
        void Debug(object data);
        void Error(string data);
        void Assertion(string name, bool result, string errorTxt);
    }
}
