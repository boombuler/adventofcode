using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode
{
    interface ISolution
    {
        int Year { get; }
        int Day { get; }
        void Run();

        bool Validate();
    }
}
