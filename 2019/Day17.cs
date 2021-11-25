using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2019
{
    class Day17 : Solution
    {
        private static char[] DirectionImages = new char[] { '^', '<', 'v', '>' };
        private static Point2D[] Directions = new Point2D[] { (0, -1), (-1, 0), (0, 1), (1, 0) };

        private Dictionary<Point2D, char> GetScaffoldMap()
        {
            var vm = new IntCodeVM(Input);
            return vm.RunASCIICommands().Select(l => l.Result)
                .SelectMany((line, y) => line.Select((c, x) => (new Point2D(x, y), c)))
                .Where(t => t.c != '.')
                .ToDictionary(t => t.Item1, t => t.c);
        }

        private IEnumerable<string> Path()
        {
            var map = GetScaffoldMap();
            var startingPoint = map.First(kvp => kvp.Value != '#');
            var position = startingPoint.Key;
            var dir = Array.IndexOf(DirectionImages, startingPoint.Value);
            int steps = 0;
            int Left() => (dir + 1) % 4;
            int Right() => (dir + 3) % 4;

            while (true)
            {
                if (map.ContainsKey(position + Directions[dir]))
                {
                    steps++;
                    position += Directions[dir];
                    continue;
                }
                if (steps > 0)
                    yield return steps.ToString();
                steps = 0;

                if (map.ContainsKey(position + Directions[Left()]))
                {
                    yield return "L";
                    dir = Left();
                }
                else if (map.ContainsKey(position + Directions[Right()]))
                {
                    yield return "R";
                    dir = Right();
                }
                else
                    yield break;
            }
        }

        private int StrLen(IEnumerable<string> function)
            => Math.Max(function.Aggregate(0, (sum, s) => sum + 1 + s.Length) - 1, 0);

        private ImmutableList<string> ReplaceFunction(string name, IEnumerable<string> function, ImmutableList<string> path)
        {
            var fnLen = function.Count();

            bool replaced = true;
            while (replaced)
            {
                replaced = false;
                for (int idx = 0; idx < path.Count; idx++)
                {
                    if (path.Skip(idx).Take(fnLen).SequenceEqual(function))
                    {
                        path = path.RemoveRange(idx, fnLen).Insert(idx, name);
                        replaced = true;
                    }
                }
            }
            return path;
        }

        private (ImmutableList<string> Main, ImmutableList<IEnumerable<string>> Functions)? ExctractFunctions(ImmutableList<string> path, int fnIdx = 0)
        {
            string[] Functions = new[] { "A", "B", "C" };

            int nextFn = fnIdx + 1;
            int idx = path.Select((f, i) => (Value: f, Index: i)).First(i => !Functions.Contains(i.Value)).Index;
            int len = 1;
            while (StrLen(path.Skip(idx).Take(len)) <= 20)
            {
                var fn = path.Skip(idx).Take(len);
                if (fn.Any(Functions.Contains))
                    return null;

                var data = ReplaceFunction(Functions[fnIdx], fn, path);

                if (nextFn == Functions.Length)
                {
                    if (data.All(Functions.Contains))
                        return (data, ImmutableList<IEnumerable<string>>.Empty.Add(fn));
                }
                else
                {
                    var res = ExctractFunctions(data, nextFn);
                    if (res.HasValue)
                    {
                        var (main, functs) = res.Value;
                        return (main, functs.Insert(0, fn));
                    }
                }
                len++;
            }
            return null;
        }

        protected override long? Part1()
        {
            var scaffolds = GetScaffoldMap();

            return scaffolds.Keys
                .Where(p => !Directions.Select(d => d + p).Any(d => !scaffolds.ContainsKey(d)))
                .Sum(p => p.X * p.Y);
        }

        protected override long? Part2()
        {
            var program = ExctractFunctions(Path().ToImmutableList());
            if (!program.HasValue)
            {
                Error("No solution -.-");
                return null;
            }

            var (main, funcs) = program.Value;
            var code = new StringBuilder();
            code.Append(string.Join(",", main)).Append('\n');
            code.Append(string.Join("\n", funcs.Select(f => string.Join(",", f)))).Append('\n');
            code.Append('n').Append('\n');
            var intCode = Encoding.ASCII.GetBytes(code.ToString())
                .Select(b => (long)b)
                .ToArray();

            return new IntCodeVM(Input).SetAddress(0, 2).Run(intCode).Last();
        }
    }
}
