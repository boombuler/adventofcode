using AdventHelper;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _2016_14
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run(); 
        
        class HashBox
        {
            static readonly ImmutableArray<string> fHexLookup;

            private readonly Dictionary<int, string> fHashes = new Dictionary<int, string>();
            private readonly string fSalt;
            private readonly int fRounds;
            
            public HashBox(string salt, int rounds)
                => (fSalt, fRounds) = (salt, rounds);
            
            static HashBox()
            {
                fHexLookup = Enumerable.Range(0, 256)
                    .Select(b => b.ToString("x2"))
                    .ToImmutableArray();
            }

            private static string DoHash(string value, int rounds)
            {
                var md = MD5.Create();
                return Enumerable.Range(0, rounds).Aggregate(value, (val, _) =>
                    string.Concat(
                        md.ComputeHash(Encoding.ASCII.GetBytes(val)).Select(b => fHexLookup[b])
                    )
                );
            }
        
            public string Get(int number)
            {
                if (fHashes.TryGetValue(number, out string hash))
                    return hash;

                var newHashes = Enumerable.Range(number, 10_000)
                    .Select(n => new { Index = n, Value = fSalt + n, Rounds = fRounds })
                    .AsParallel()
                    .Select(itm => new { Index = itm.Index, Hash = DoHash(itm.Value, itm.Rounds) });

                foreach (var h in newHashes)
                    fHashes[h.Index] = h.Hash;
                return fHashes[number];
            }
        }

        private static Regex FindTriplett = new Regex(@"([0-9a-f])\1{2}", RegexOptions.Compiled);
       
        private IEnumerable<int> GenerateKeys(string salt, int additionalRounds)
        {
            const int LOOKAHEAD = 1_000;
            var box = new HashBox(salt, additionalRounds + 1);

            return EnumerableHelper.Generate()
                .Select(i => new { Index = i, Match = FindTriplett.Match(box.Get(i)) })
                .Where(m => m.Match.Success)
                .Where(m =>
                    Enumerable.Range(m.Index + 1, LOOKAHEAD).Select(box.Get)
                        .Any(new Regex(m.Match.Value[0] + "{5}").IsMatch)
                ).Select(m => m.Index);
        }

        private long GetIndexForKeyNo(string salt, int keyNo, int additionalRounds = 0)
            => GenerateKeys(salt, additionalRounds).Skip(keyNo - 1).First();


        private const string INPUT = "cuanljph";
        protected override long? Part1()
        {
            Assert(GetIndexForKeyNo("abc", 1), 39);
            Assert(GetIndexForKeyNo("abc", 64), 22728);
            return GetIndexForKeyNo(INPUT, 64);
        }

        protected override long? Part2()
        {
            Assert(GetIndexForKeyNo("abc", 1, 2016), 10);
            Assert(GetIndexForKeyNo("abc", 64, 2016), 22551);
            return GetIndexForKeyNo(INPUT, 64, 2016);
        }
    }
}
