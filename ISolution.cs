using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode
{
    interface ISolution
    {
        int Year { get; }
        int Day { get; }

        string Part1(IOutput output);
        string Part2(IOutput output);
    }
}
