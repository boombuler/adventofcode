using AdventHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2015_09
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        class Instruction 
        { 
            public string from; 
            public string to; 
            public long dist; 
        }
        private static readonly Func<string, Instruction> ParseInstruction
            = new Regex(@"(?<from>\w+) to (?<to>\w+) = (?<dist>\d+)", RegexOptions.Compiled).ToFactory<Instruction>();

        private IEnumerable<(string from, string to, long dist)> ReadInstructions(string fileName)
        {
            foreach(var line in ReadLines(fileName))
            {
                var itm = ParseInstruction(line);
                yield return (itm.from, itm.to, itm.dist);
                yield return (itm.to, itm.from, itm.dist);
            }
        }

        private IEnumerable<long> PermutateRoutes(string fileName)
        {
            var instructions = ReadInstructions(fileName).ToList();

            var cities = instructions.Select(i => i.from).Distinct();
            var distances = instructions.ToDictionary(v => (v.from, v.to), v => v.dist);

            foreach(var route in cities.Permuatate())
            {
                long val = 0;
                bool valid = true;
                for (int i = 1; i < route.Length; i++)
                {
                    if (distances.TryGetValue((route[i - 1], route[i]), out var d))
                        val += d;
                    else
                    {
                        valid = false;
                        break;
                    }
                    if (!valid)
                        break;
                }
                if (valid)
                    yield return val;
            }
        }

        protected override long? Part1()
        {
            Assert(PermutateRoutes("Sample.txt").Min(), 605);
            return PermutateRoutes("Input.txt").Min();
        }

        protected override long? Part2()
        {
            Assert(PermutateRoutes("Sample.txt").Max(), 982);
            return PermutateRoutes("Input.txt").Max();
        }
    }
}
