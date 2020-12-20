using AdventHelper;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace _2015_08
{
    class Program : ProgramBase
    {
        private const string SAMPLE = "Sample";
        private const string INPUT = "Input";
        static void Main(string[] args) => new Program().Run();

        private long CountChars(string file)
            => ReadLines(file).Select(s => (long)s.Trim().Length).Sum();
        private long CountUnquoted(string file)
        {
            long sum = 0;
            foreach(var l in ReadLines(file))
            {
                string line = l.Trim();
                sum += line.Length - 2;
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '\\')
                    {
                        sum--;
                        if (line[++i] == 'x')
                        {
                            i += 2;
                            sum -= 2;
                        }
                    }
                }
            }

            return sum;
        }

        private long ContQuoted(string s)
        {
            long sum = 2;
            foreach (char c in s)
            {
                if (c == '"' || c == '\\')
                    sum++;
                sum++;
            }
            return sum;
        }

        private long CountQuotedStringChars(string file)
            => ReadLines(file).Select(x => ContQuoted(x.Trim())).Sum();

        protected override long? Part1()
        {
            Assert(CountChars(SAMPLE), 23);
            Assert(CountUnquoted(SAMPLE), 11);
            return CountChars(INPUT) - CountUnquoted(INPUT);
        }

        protected override long? Part2()
        {
            Assert(CountQuotedStringChars(SAMPLE), 42);
            return CountQuotedStringChars(INPUT) - CountChars(INPUT);
        }
    }
}
