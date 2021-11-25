using System.Linq;
using System.Text;

namespace AdventOfCode._2019
{
    class Day21 : Solution
    {
        private long RunCode(params string[] springscript)
            => long.Parse(new IntCodeVM(Input).RunASCIICommands(springscript).Last().Result);


        protected override long? Part1()
        {   // !A && ((!B || !C) && D)
            return RunCode(
                "NOT B T",
                "NOT C J",
                "OR T J",
                "AND D J",
                "NOT A T",
                "OR T J",
                "WALK");
        }

        protected override long? Part2()
        {   // !A && ((!B || !C) && D && H)
            return RunCode(
                "NOT B T",
                "NOT C J",
                "OR T J",
                "AND D J",
                "AND H J",
                "NOT A T",
                "OR T J",
                "RUN");
        }
    }
}
