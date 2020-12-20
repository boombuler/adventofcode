using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2015_06
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        abstract class GridBase<T>
        {
            private static readonly Regex ParseCommand = new Regex(@"(?<action>turn on|turn off|toggle) (?<xMin>\d+),(?<yMin>\d+) through (?<xMax>\d+),(?<yMax>\d+)", RegexOptions.Compiled);
            private const int SIZE = 1000;
            private T[,] Lights = new T[SIZE, SIZE];

            private void Set((int x, int y) p1, (int x, int y) p2, bool? command)
            {
                (int min, int max) Order(int i1, int i2) 
                    => i1 < i2 ? (i1, i2) : (i2, i1);

                (int xMin, int xMax) = Order(p1.x, p2.x);
                (int yMin, int yMax) = Order(p1.y, p2.y);

                for (int x = xMin; x <= xMax; x++)
                    for (int y = yMin; y <= yMax; y++)
                        Lights[x, y] = Apply(Lights[x, y], command);
            }

            protected abstract T Apply(T oldValue, bool? command);
            protected abstract int ValueOf(T val);

            public int Handle(IEnumerable<string> commands)
            {
                foreach (var command in commands)
                {
                    var m = ParseCommand.Match(command);
                    bool? action = m.Groups["action"].Value switch
                    {
                        "turn on" => (bool?)true,
                        "turn off" => (bool?)false,
                        _ => (bool?)null
                    };
                    var min = (int.Parse(m.Groups["xMin"].Value), int.Parse(m.Groups["yMin"].Value));
                    var max = (int.Parse(m.Groups["xMax"].Value), int.Parse(m.Groups["yMax"].Value));
                    Set(min, max, action);
                }
                return Lights.Cast<T>().Select(ValueOf).Sum();
            }
        }

        class BoolGrid : GridBase<bool>
        {
            protected override int ValueOf(bool val) => val ? 1 : 0;
            protected override bool Apply(bool oldValue, bool? command)
                => command ?? !oldValue;
        }
        class IntGrid : GridBase<int>
        {
            protected override int ValueOf(int val) => val;
            protected override int Apply(int oldValue, bool? command)
                => Math.Max(0, oldValue + (command switch { true => 1, false => -1, _ => 2 }));
        }

        protected override long? Part1() => new BoolGrid().Handle(ReadLines("Input"));
        protected override long? Part2() => new IntGrid().Handle(ReadLines("Input"));
    }
}
