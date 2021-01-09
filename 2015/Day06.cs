using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2015
{
    class Day06 : Solution
    {
        abstract class GridBase<T>
        {
            private static readonly Regex ParseCommand = new Regex(@"(?<action>turn on|turn off|toggle) (?<xMin>\d+),(?<yMin>\d+) through (?<xMax>\d+),(?<yMax>\d+)", RegexOptions.Compiled);
            private const int SIZE = 1000;
            private T[,] Lights = new T[SIZE, SIZE];

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
                    var min = new Point2D(long.Parse(m.Groups["xMin"].Value), long.Parse(m.Groups["yMin"].Value));
                    var max = new Point2D(long.Parse(m.Groups["xMax"].Value), long.Parse(m.Groups["yMax"].Value));
                    foreach (var p in Point2D.Range(min, max))
                        Lights[p.X, p.Y] = Apply(Lights[p.X, p.Y], action);
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

        protected override long? Part1() => new BoolGrid().Handle(Input.Lines());
        protected override long? Part2() => new IntGrid().Handle(Input.Lines());
    }
}
