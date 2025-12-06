namespace AdventOfCode._2016;

using System.Security.Cryptography;

class Day05 : Solution<string>
{
    static bool IsValidHash(ReadOnlySpan<byte> hash)
        => hash[0] == 0 && hash[1] == 0 && (hash[2] & 0xF0) == 0;

    private static IEnumerable<byte[]> GenerateValidHashes(string doorId)
    {
        var md = MD5.Create();
        var inputBuf = Encoding.ASCII.GetBytes(doorId);
        int prefixLen = inputBuf.Length;
        Array.Resize(ref inputBuf, inputBuf.Length + 12);
        inputBuf[prefixLen] = (byte)'0';
        var counterLen = 1;
        
        var hashBuf = new byte[md.HashSize / 8];
        while (true)
        {
            md.TryComputeHash(inputBuf.AsSpan(0, prefixLen + counterLen), hashBuf, out _);

            if (IsValidHash(hashBuf))
                yield return (byte[])hashBuf.Clone();

            AsciiCounter.Step(inputBuf.AsSpan(prefixLen), ref counterLen);
        }
    }

    private static string GetPassword(string doorId)
        => string.Concat(GenerateValidHashes(doorId).Select(hash => (hash[2] & 0x0F).ToString("x")).Take(8));

    private static string GetPassword2(string doorId)
    {
        char?[] pwd = new char?[8];

        foreach (var hash in GenerateValidHashes(doorId))
        {
            int pos = hash[2] & 0x0F;
            if (pos < 8 && !pwd[pos].HasValue)
            {
                int val = (hash[3] & 0xF0) >> 4;
                pwd[pos] = val.ToString("x")[0];

                if (pwd.All(c => c.HasValue))
                    return new string([.. pwd.Select(c => c.Value)]);
            }
        }
        throw new InvalidDataException("No valid solution");
    }

    protected override string Part1()
    {
        Assert(GetPassword("abc"), "18f47a30");
        return GetPassword(Input);
    }

    protected override string Part2()
    {
        Assert(GetPassword2("abc"), "05ace8e3");
        return GetPassword2(Input);
    }
}
