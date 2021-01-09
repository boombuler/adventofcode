using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode._2016
{
    class Day09 : Solution
    {

        private long DecompressV1(string input) => Decompress(input, false);
        private long DecompressV2(string input) => Decompress(input, true);
        private long Decompress(string input, bool recursive)
        {
            long result = 0;

            using(var sr = new StringReader(input))
            {
                while (sr.TryRead(out char c))
                {
                    if (c != '(')
                    {
                        result++;
                        continue;
                    }

                    var chars = int.Parse(sr.ReadToTerm('x'));
                    var rpt = int.Parse(sr.ReadToTerm(')'));

                    var subStr = new StringBuilder(chars);
                    for (int i = 0; i < chars; i++)
                        subStr.Append((char)sr.Read());

                    if (recursive)
                        result += rpt * Decompress(subStr.ToString(), true);
                    else
                        result += rpt * chars;
                }
            }
            return result;
        }

        protected override long? Part1()
        {
            Assert(DecompressV1("ADVENT"), 6);
            Assert(DecompressV1("A(1x5)BC"), 7);
            Assert(DecompressV1("(3x3)XYZ"), 9);
            Assert(DecompressV1("A(2x2)BCD(2x2)EFG"), 11);
            Assert(DecompressV1("(6x1)(1x3)A"), 6);
            Assert(DecompressV1("X(8x2)(3x3)ABCY"), 18);

            return DecompressV1(Input);
        }
    
        protected override long? Part2()
        {
            Assert(DecompressV2("(3x3)XYZ"), 9);
            Assert(DecompressV2("X(8x2)(3x3)ABCY"), 20);
            Assert(DecompressV2("(27x12)(20x12)(13x14)(7x10)(1x12)A"), 241920);
            Assert(DecompressV2("(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN"), 445);
            return DecompressV2(Input);
        }
    }
}
