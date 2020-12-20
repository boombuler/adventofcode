using AdventHelper;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2015_05
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private static readonly Regex Rule1 = new Regex(@"(.*[aeiou]){3}", RegexOptions.Compiled);
        private static readonly Regex Rule2 = new Regex(@"(\w)\1", RegexOptions.Compiled);
        private static readonly Regex Rule3 = new Regex(@"^((?!(ab|cd|pq|xy)).)*$", RegexOptions.Compiled);

        private static readonly Regex Rule4 = new Regex(@"(\w\w).*\1", RegexOptions.Compiled);
        private static readonly Regex Rule5 = new Regex(@"(\w)\w\1", RegexOptions.Compiled);

        private bool IsNice(string input)
            => Rule1.IsMatch(input) && Rule2.IsMatch(input) && Rule3.IsMatch(input);
        private bool IsNice2(string input)
            => Rule4.IsMatch(input) && Rule5.IsMatch(input);

        protected override long? Part1()
        {
            Assert(IsNice("ugknbfddgicrmopn"));
            Assert(IsNice("aaa"));
            Assert(!IsNice("jchzalrnumimnmhp"));
            Assert(!IsNice("haegwjzuvuyypxyu"));
            Assert(!IsNice("dvszwmarrgswjxmb"));
            return ReadLines("Input").Where(IsNice).Count();
        }
        protected override long? Part2()
        {
            Assert(IsNice2("xyxy"));
            Assert(!IsNice2("aaa"));
            Assert(IsNice2("qjhvhtzxzqqjkmpb"));
            Assert(IsNice2("xxyxx"));
            Assert(!IsNice2("uurcxstgmygtbstg"));
            Assert(!IsNice2("ieodomkazucvgmuy"));
            return ReadLines("Input").Where(IsNice2).Count();
        }
    }
}
