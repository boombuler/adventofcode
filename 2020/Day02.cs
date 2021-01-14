using AdventOfCode.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode._2020
{
    class Day02 : Solution
    {
        record PasswordLine(int i1, int i2, char c, string test);
        private static readonly Func<string, PasswordLine> ParsePasswordLine = 
            new Regex(@"(?<i1>\d+)\-(?<i2>\d+)\W(?<c>\w):\W(?<test>\w+)", RegexOptions.Compiled).ToFactory<PasswordLine>();

        static bool IsPasswordValidPart1(string line)
        {
            (int lowerBound, int upperBound, char testChar, string test) = ParsePasswordLine(line);
            var testCharCount = test.Where(c => c == testChar).Count();
            return (testCharCount >= lowerBound) && (testCharCount <= upperBound);
        }

        static bool IsPasswordValidPart2(string line)
        {
            (int i1, int i2, char c, string test) = ParsePasswordLine(line);
            bool checkIndex(int i) => test[i - 1] == c;
            return checkIndex(i1) ^ checkIndex(i2);
        }
        protected override long? Part1() => Input.Lines().Where(IsPasswordValidPart1).Count();
        protected override long? Part2() => Input.Lines().Where(IsPasswordValidPart2).Count();
    }
}
