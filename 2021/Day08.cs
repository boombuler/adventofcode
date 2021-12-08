using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode._2021
{
    class Day08 : Solution
    {
        private string[] DigitCodes = new[]
        {
            "abcefg", "cf", "acdeg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg"
        };

        private Func<string, int> GetTranslatorFromNotes(string notes)
        {
            var valid = new HashSet<string>(DigitCodes);
            var samples = notes.Split(' ').OrderBy(l => l.Length).ToList();
            const string ALPHABET = "abcdefg";
            foreach (var solution in ALPHABET.Permuatate())
            {
                string Translate(string s) => new string(s.Select(c => ALPHABET[Array.IndexOf(solution, c)]).OrderBy(c => c).ToArray());
                if (samples.Select(Translate).All(valid.Contains))
                    return (s) => Array.IndexOf(DigitCodes, Translate(s));
            }
            return null;
        }

        private long GetOutput(string line)
        {
            var (notes, (digits, _)) = line.Split('|').Select(n => n.Trim());
            var translator = GetTranslatorFromNotes(notes);
            return digits.Split(' ').Select(translator).Aggregate((sum, d) => sum * 10 + d);
        }

        protected override long? Part1()
            => Input.Lines().SelectMany(l => l.Split('|').Last().Trim().Split(' '))
                .Select(s => s.Length).Count(x => x != 6 && x != 5);

        protected override long? Part2() => Input.Lines().Sum(GetOutput);
    }
}
