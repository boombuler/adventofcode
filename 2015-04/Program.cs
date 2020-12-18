using AdventHelper;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace _2015_04
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        private long FindLowestHash(string privateKey, int leadingZeros = 5)
        {
            var md5 = MD5.Create();

            string prefix = string.Empty;
            for (int i = 0; i < leadingZeros; i++)
            {
                if (i % 2 == 0 && i > 0)
                    prefix += '-';
                prefix += '0';
                
            }

            int count = 0;
            while (true)
            {
                var h = md5.ComputeHash(Encoding.ASCII.GetBytes(privateKey + Convert.ToString(count)));
                if (BitConverter.ToString(h).StartsWith(prefix))
                    return count;
                else
                    count++;
            }
        }

        protected override long? Part1()
        {
            Assert(FindLowestHash("abcdef"), 609043);
            Assert(FindLowestHash("pqrstuv"), 1048970);
            return FindLowestHash("iwrupvqb");
        }

        protected override long? Part2() => FindLowestHash("iwrupvqb", 6);
    }
}
