using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode._2015
{
    class Day04 : Solution
    {
        private void IncText(Span<byte> txt, ref int len)
        {
            var idx = txt.Length-1;
            while (true)
            {
                txt[idx]++;
                if (txt[idx] > (byte)'9')
                    txt[idx--] = (byte)'0';
                else
                {
                    var l = txt.Length - idx;
                    if (l > len)
                        len = l;
                    return;
                }   
            }
        }

        private long FindLowestHash(string privateKey, int leadingZeros = 5)
        {
            var md5 = MD5.Create();

            bool IsMatch()
            {
                for (int i = 0; i < leadingZeros / 2; i++)
                {
                    if (md5.Hash[i] != 0)
                        return false;
                }
                if (leadingZeros % 2 != 0)
                {
                    var v = md5.Hash[leadingZeros / 2];
                    return (v & 0xF0) == 0;
                }
                return true;
            }

            var pk = Encoding.ASCII.GetBytes(privateKey);
            var counter = Enumerable.Range(0, 15).Select(_ => (byte)'0').ToArray();
            int len = 1;
            while (true)
            {
                md5.TransformBlock(pk, 0, pk.Length, null, 0);
                md5.TransformFinalBlock(counter, counter.Length - len, len);
                if (IsMatch())
                {
                    var txt = Encoding.ASCII.GetString(counter, counter.Length - len, len);
                    return long.Parse(txt);
                }
                else
                    IncText(counter, ref len);
            }
        }

        protected override long? Part1()
        {
            Assert(FindLowestHash("abcdef"), 609043);
            Assert(FindLowestHash("pqrstuv"), 1048970);
            return FindLowestHash(Input);
        }

        protected override long? Part2() => FindLowestHash(Input, 6);
    }
}
