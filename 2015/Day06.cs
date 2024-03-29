﻿namespace AdventOfCode._2015;

using static Parser;

class Day06 : Solution
{
    abstract class GridBase<T>
    {
        private static readonly Func<string, (bool? Action, Point2D<int> Min, Point2D<int> Max)> ParseCommand =
            from cmd in Str("turn on", (bool?)true) | Str("turn off", (bool?)false) | Str("toggle", (bool?)null)
            from min in IntPoint2D.Token() + "through"
            from max in IntPoint2D.Token()
            select (cmd, min, max);

        protected abstract void Apply(ref T value, bool? command);
        protected abstract int ValueOf(T val);

        public int Handle(string input)
        {
            var commands = input.Lines().Select(ParseCommand);
            const int SIZE = 1000;
            var lights = new T[SIZE * SIZE];

            foreach (var (act, min, max) in commands)
            {
                foreach (var p in Point2D<int>.Range(min, max))
                    Apply(ref lights[p.Y * SIZE + p.X], act);
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
