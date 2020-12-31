﻿using AdventHelper;
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
            char[] buffer = new char[length];
            Array.Copy(input.ToCharArray(), buffer, input.Length);

            int i = input.Length;
            while (i < length)
            {
                int aEnd = i-1;
                buffer[i++] = '0';
                int stop = aEnd - Math.Min(length - 1 - i, aEnd);
                
                for (int bi = aEnd; bi >= stop; bi--)
                    buffer[i++] = buffer[bi] == '0' ? '1' : '0';
            }

            while ((length & 1) == 0)
            {
                for (i = 0; i < length; i += 2)
                    buffer[i >> 1] = (buffer[i] == buffer[i + 1]) ? '1' : '0';
                length >>= 1;
            }
            return new string(buffer, 0, length);
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