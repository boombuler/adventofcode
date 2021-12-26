namespace AdventOfCode._2015;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

class Day06 : Solution
{
    abstract class GridBase<T>
    {
        record Command(string Action, int XMin, int XMax, int YMin, int YMax);
        private static readonly Func<string, Command> ParseCommand = new Regex(@"(?<Action>turn on|turn off|toggle) (?<XMin>\d+),(?<YMin>\d+) through (?<XMax>\d+),(?<YMax>\d+)", RegexOptions.Compiled).ToFactory<Command>();
            
        protected abstract void Apply(ref T value, bool? command);
        protected abstract int ValueOf(T val);

        public int Handle(string input)
        {
            var commands = input.Lines().Select(ParseCommand);
            const int SIZE = 1000;
            var lights = new T[SIZE * SIZE];

            foreach (var c in commands)
            {
                bool? action = c.Action switch
                {
                    "turn on" => (bool?)true,
                    "turn off" => (bool?)false,
                    _ => (bool?)null
                };

                for (int y = c.YMin; y <= c.YMax; y++)
                {
                    for (int x = c.XMin; x <= c.XMax; x++)
                    {
                        Apply(ref lights[y*SIZE+x], action);
                    }
                }
            }
            return lights.Sum(ValueOf);
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
