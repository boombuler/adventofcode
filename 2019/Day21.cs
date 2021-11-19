using System.Linq;
using System.Text;

namespace AdventOfCode._2019
{
    class Day21 : Solution
    {
        private long? RunCode(params string[] springscript)
        {
            var vm = new IntCodeVM(Input);
            var result = vm.Run(springscript.SelectMany(s => s + "\n").Select(b => (long)(byte)b).ToArray());
            var sb = new StringBuilder();
            foreach(var c in result)
            {
                if (c > 127)
                    return c;
                sb.Append((char)(byte)c);
            }
            Debug(sb.ToString());
            return null;
        }


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
