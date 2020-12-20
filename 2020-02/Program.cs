using AdventHelper;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2020_02
{
    class Program : ProgramBase
    {
        private static readonly Regex passwordLine = new Regex(@"(?<i1>\d+)\-(?<i2>\d+)\W(?<char>\w):\W(?<test>\w+)", RegexOptions.Compiled);

        static (int i1, int i2, char c, string test) ParsePasswordLine(string line)
        {
            var match = passwordLine.Match(line);
            if (!match.Success)
                throw new ArgumentException("Invalid Passwordline");
            return (
                Convert.ToInt32(match.Groups["i1"].Value),
                Convert.ToInt32(match.Groups["i2"].Value),
                match.Groups["char"].Value[0],
                match.Groups["test"].Value
            );
        }


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
        protected override long? Part1() => ReadLines("Input").Where(IsPasswordValidPart1).Count();
        protected override long? Part2() => ReadLines("Input").Where(IsPasswordValidPart2).Count();
    
        static void Main(string[] args) => new Program().Run();
    }
}
