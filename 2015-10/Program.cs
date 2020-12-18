using AdventHelper;
using System;
using System.Linq;
using System.Text;

namespace _2015_10
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();
        
        private const string PuzzeInput = "1113222113";

        private string Generate(string input)
        {
            char cur = input.First();
            int count = 1;
            var result = new StringBuilder();
            foreach(char c in input.Skip(1))
            {
                if (c == cur)
                    count++;
                else
                {
                    result.Append(count).Append(cur);
                    count = 1;
                    cur = c;
                }
            }
            result.Append(count).Append(cur);
            return result.ToString();
        }

        private string Generate(string input, int times)
            => Enumerable.Range(0, times).Aggregate(input, (cur, _) => Generate(cur));
        protected override long? Part1()
        {
            Assert(Generate("1", 5), "312211");
            return Generate(PuzzeInput, 40).Length;
        }

        protected override long? Part2() => Generate(PuzzeInput, 50).Length;
    }
}
