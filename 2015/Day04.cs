namespace AdventOfCode._2015;

using System.Runtime.InteropServices;
using System.Security.Cryptography;

class Day04 : Solution
{
    private long FindLowestHash(string privateKey, int mask = 0xF0FFFF)
    {
        var md = MD5.Create(); 
        var pk = Encoding.ASCII.GetBytes(privateKey);
        int prefixLen = pk.Length;
        Array.Resize(ref pk, pk.Length + 12);

        var counter = new AsciiCounter(pk.AsSpan(prefixLen));
        Span<byte> hash = stackalloc byte[md.HashSize];
        while (true)
        {
            md.TryComputeHash(pk.AsSpan(0, prefixLen + counter.Length), hash, out _);
            
            if ((MemoryMarshal.Read<int>(hash) & mask) == 0)
                return counter.Value;
            counter.Step();
        }
    }

    protected override long? Part1()
    {
        Assert(FindLowestHash("abcdef"), 609043);
        Assert(FindLowestHash("pqrstuv"), 1048970);
        return FindLowestHash(Input);
    }

    protected override long? Part2() => FindLowestHash(Input, 0xFFFFFF);
}
