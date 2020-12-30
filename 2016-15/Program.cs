using AdventHelper;
using System;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace _2016_15
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        class Disc { public int No; public int Positions; public int Offset; }
        private static Func<string, Disc> ParseDisc = new Regex(@"Disc #(?<No>\d+) has (?<Positions>\d+) positions; at time=0, it is at position (?<Offset>\d+)\.", RegexOptions.Compiled)
            .ToFactory<Disc>();

        public long GetButtonPressTime(string discFile)
        {
            var discs = ReadLines(discFile).Select(ParseDisc);
            return (long)discs
                .Select(d => (a: -(BigInteger)d.Offset - d.No, n: (BigInteger)d.Positions))
                .Aggregate(ChineseRemainder.Solve).a;
        }

        protected override long? Part1()
        {
            Assert(GetButtonPressTime("Sample"), 5);
            return GetButtonPressTime("Input");
        }

        protected override long? Part2() => GetButtonPressTime("Input2");
    }
}
