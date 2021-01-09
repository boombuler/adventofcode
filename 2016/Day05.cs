using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace AdventOfCode._2016
{
    class Day05 : Solution<string>
    {
        private string GetPassword(string doorId)
        {   
            string pwd = string.Empty;
            long t = 0;
            var md5 = MD5.Create();
            while (pwd.Length < 8)
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(doorId + (t++).ToString()));
                if (!hash.Take(2).Any(x => x != 0))
                {
                    var hashStr = BitConverter.ToString(hash).Replace("-", string.Empty);
                    if (hashStr.StartsWith("00000"))
                        pwd = pwd + hashStr[5];
                }
            }
            return pwd.ToLowerInvariant();
        }

        private string GetPassword2(string doorId)
        {
            char?[] pwd = new char?[8];
            long t = 0;
            var md5 = MD5.Create();

            while (true)
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(doorId + (t++).ToString()));
                if (!hash.Take(2).Any(x => x != 0))
                {
                    var hashStr = BitConverter.ToString(hash).Replace("-", string.Empty);
                    if (hashStr.StartsWith("00000"))
                    {
                        var pos = int.Parse(hashStr.Substring(5, 1), System.Globalization.NumberStyles.HexNumber);
                        if (pos < 8 && !pwd[pos].HasValue)
                        {
                            pwd[pos] = hashStr[6];
                            if (!pwd.Any(c => !c.HasValue))
                                return new string(pwd.Select(c => c.Value).ToArray()).ToLowerInvariant();
                        }
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
