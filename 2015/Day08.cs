using AdventOfCode.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode._2015
{
    class Day08 : Solution
    {
        private long CountChars(string input)
            => input.Lines().Select(s => (long)s.Trim().Length).Sum();

        private long CountUnquoted(string input)
        {
            long sum = 0;
            foreach(var l in input.Lines())
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

        private long CountQuoted(string s)
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

        private long CountQuotedStringChars(string input)
            => input.Lines().Select(x => CountQuoted(x.Trim())).Sum();

        protected override long? Part1()
        {
            Assert(CountChars(Sample()), 23);
            Assert(CountUnquoted(Sample()), 11);
            return CountChars(Input) - CountUnquoted(Input);
        }

        protected override long? Part2()
        {
            Assert(CountQuotedStringChars(Sample()), 42);
            return CountQuotedStringChars(Input) - CountChars(Input);
        }
    }
}
