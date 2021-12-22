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
            record Command(string Action, int xMin, int xMax, int yMin, int yMax);
            private static readonly Func<string, Command> ParseCommand = new Regex(@"(?<Action>turn on|turn off|toggle) (?<xMin>\d+),(?<yMin>\d+) through (?<xMax>\d+),(?<yMax>\d+)", RegexOptions.Compiled).ToFactory<Command>();
            
            protected abstract void Apply(ref T value, bool? command);
            protected abstract int ValueOf(T val);

            public int Handle(string input)
            {
                var commands = input.Lines().Select(ParseCommand);
                const int SIZE = 1000;
                var Lights = new T[SIZE * SIZE];


                foreach (var c in commands)
                {
                    bool? action = c.Action switch
                    {
                        "turn on" => (bool?)true,
                        "turn off" => (bool?)false,
                        _ => (bool?)null
                    };

                    for (int y = c.yMin; y <= c.yMax; y++)
                    {
                        for (int x = c.xMin; x <= c.xMax; x++)
                        {
                            Apply(ref Lights[y*SIZE+x], action);
                        }
                    }
                }
                return Lights.Sum(ValueOf);
            }
        }

        class BoolGrid : GridBase<bool>
        {
            protected override int ValueOf(bool val) => val ? 1 : 0;
            protected override void Apply(ref bool value, bool? command)
                => value = command ?? !value;
        }
        class IntGrid : GridBase<int>
        {
            protected override int ValueOf(int val) => val;
            protected override void Apply(ref int value, bool? command)
                => value = Math.Max(0, value + (command switch { true => 1, false => -1, _ => 2 }));
        }

        protected override long? Part1() => new BoolGrid().Handle(Input);
        protected override long? Part2() => new IntGrid().Handle(Input);
    }
}
