namespace AdventOfCode._2015;

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using AdventOfCode.Utils;

class Day04 : Solution
{
    private long FindLowestHash(string privateKey, int leadingZeros = 5)
    {
        bool IsMatch(byte[] hash)
        {
            for (int i = 0; i < leadingZeros / 2; i++)
            {
                if (hash[i] != 0)
                    return false;
            }
            if (leadingZeros % 2 != 0)
            {
                var v = hash[leadingZeros / 2];
                return (v & 0xF0) == 0;
            }
            return true;
        }

        var sw = Stopwatch.StartNew();
        try
        {
            var md = MD5.Create();
            var pk = Encoding.ASCII.GetBytes(privateKey);
            int prefixLen = pk.Length;
            Array.Resize(ref pk, pk.Length + 12);

            var counter = new AsciiCounter(pk.AsSpan(prefixLen));
            while (true)
            {
                var hash = md.ComputeHash(pk, 0, prefixLen + counter.Length);
                if (IsMatch(hash))
                    return counter.Value;
                counter.Step();
            }
        }
        finally
        {
            sw.Stop();
            Debug(sw.Elapsed);
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
