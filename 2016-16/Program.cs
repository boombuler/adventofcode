using AdventHelper;
using System;
using System.Linq;
using System.Text;

namespace _2016_16
{
    class Program : ProgramBase<string>
    {
        static void Main(string[] args) => new Program().Run();

        

        private string GenerateDataGetCheckSum(string input, int length)
        {
            var sb = new StringBuilder(input, length);
            while (sb.Length < length)
            {
                var b = sb.ToString();
                sb.Append('0');
                foreach (var c in b.Reverse())
                    sb.Append(c == '0' ? '1' : '0');
            }
            input = sb.ToString();

            while (length % 2 == 0)
            {
                sb.Clear();
                for (int i = 0; i < length; i += 2)
                    sb.Append(input[i] == input[i + 1] ? '1' : '0');
                input = sb.ToString();
                length = input.Length;
            }
            return input;
        }

        const string INPUT = "00101000101111010";

        protected override string Part1()
        {
            Assert(GenerateDataGetCheckSum("10000", 20), "01100");
            return GenerateDataGetCheckSum(INPUT, 272);
        }

        protected override string Part2() => GenerateDataGetCheckSum(INPUT, 35_651_584);
    }
}
