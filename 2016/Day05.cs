using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode._2016
{
    class Day05 : Solution<string>
    {
        bool IsValidHash(byte[] hash)
            => hash[0] == 0 && hash[1] == 0 && (hash[2] & 0xF0) == 0;

        private IEnumerable<byte[]> GenerateValidHashes(string doorId)
        {
            var md = MD5.Create();
            var buffer = Encoding.ASCII.GetBytes(doorId);
            int prefixLen = buffer.Length;
            Array.Resize(ref buffer, buffer.Length + 12);
            buffer[prefixLen] = (byte)'0';
            var counterLen = 1;

            while (true)
            {
                var hash = md.ComputeHash(buffer, 0, prefixLen + counterLen);
                if (IsValidHash(hash))
                    yield return hash;
                AsciiCounter.Step(buffer.AsSpan(prefixLen), ref counterLen);
            }
        }

        private string GetPassword(string doorId)
            => string.Concat(GenerateValidHashes(doorId).Select(hash => (hash[2] & 0x0F).ToString("x")).Take(8));

        private string GetPassword2(string doorId)
        {
            char?[] pwd = new char?[8];
            var md5 = MD5.Create();

            while (true)
            {
                foreach(var hash in GenerateValidHashes(doorId))
                {
                    int pos = hash[2] & 0x0F;
                    if (pos < 8 && !pwd[pos].HasValue)
                    {
                        int val = (hash[3] & 0xF0) >> 4;
                        pwd[pos] = val.ToString("x")[0];

                        if (pwd.All(c => c.HasValue))
                            return new string(pwd.Select(c => c.Value).ToArray());
                    }
                }
            }
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
}
