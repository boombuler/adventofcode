﻿namespace AdventOfCode._2016;

using System.Security.Cryptography;

class Day14 : Solution
{
    class HashBox
    {
        static readonly string[] fHexLookup;
        static readonly byte[] fHexCharLookup;

        private readonly Dictionary<int, string> fHashes = [];
        private readonly string fSalt;
        private readonly int fRounds;

        public HashBox(string salt, int rounds)
            => (fSalt, fRounds) = (salt, rounds);

        static HashBox()
        {
            fHexLookup = Enumerable.Range(0, 256)
                .Select(b => b.ToString("x2"))
                .ToArray();
            fHexCharLookup = Enumerable.Range(0, 16).Select(i => (byte)(i.ToString("x")[0])).ToArray();
        }

        private static string DoHash(string value, int rounds)
        {   
            Span<byte> inBuf = stackalloc byte[32];
            Span<byte> hashBuf = stackalloc byte[16];
            MD5.HashData(Encoding.ASCII.GetBytes(value), hashBuf);

            for (int r = 1; r < rounds; r++)
            {
                for (int i = 0; i < hashBuf.Length; i++)
                {
                    inBuf[i * 2 + 0] = fHexCharLookup[hashBuf[i] >> 4];
                    inBuf[i * 2 + 1] = fHexCharLookup[hashBuf[i] & 0xF];
                }
                MD5.HashData(inBuf, hashBuf);
            }
            var sb = new StringBuilder(32);
            for (int i = 0; i < hashBuf.Length; i++)
                sb.Append(fHexLookup[hashBuf[i]]);
            return sb.ToString();
        }

        public string Get(int number)
        {
            if (fHashes.TryGetValue(number, out string hash))
                return hash;

            var newHashes = Enumerable.Range(number, 1_000)
                .Select(n => new { Index = n, Value = fSalt + n, Rounds = fRounds })
                .AsParallel()
                .Select(itm => new { itm.Index, Hash = DoHash(itm.Value, itm.Rounds) });

            foreach (var h in newHashes)
                fHashes[h.Index] = h.Hash;
            return fHashes[number];
        }
    }

    private static readonly Regex FindTriplett = new(@"([0-9a-f])\1{2}", RegexOptions.Compiled);

    private static IEnumerable<int> GenerateKeys(string salt, int additionalRounds)
    {
        const int LOOKAHEAD = 1_000;
        var box = new HashBox(salt, additionalRounds + 1);

        return EnumerableHelper.Generate()
            .Select(i => new { Index = i, Match = FindTriplett.Match(box.Get(i)) })
            .Where(m => m.Match.Success)
            .Where(m =>
            {
                var search = new string(m.Match.Value[0], 5);
                return Enumerable.Range(m.Index + 1, LOOKAHEAD).Select(box.Get).Any(s => s.Contains(search));
            }).Select(m => m.Index);
    }

    private static long GetIndexForKeyNo(string salt, int keyNo, int additionalRounds = 0)
        => GenerateKeys(salt, additionalRounds).Skip(keyNo - 1).First();

    protected override long? Part1()
    {
        Assert(GetIndexForKeyNo("abc", 1), 39);
        Assert(GetIndexForKeyNo("abc", 64), 22728);
        return GetIndexForKeyNo(Input, 64);
    }

    protected override long? Part2()
    {
        Assert(GetIndexForKeyNo("abc", 1, 2016), 10);
        Assert(GetIndexForKeyNo("abc", 64, 2016), 22551);
        return GetIndexForKeyNo(Input, 64, 2016);
    }
}
