using AdventHelper;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _2015_11
{
    class Program : ProgramBase<string>
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        static void Main(string[] args) => new Program().Run();

        public string IncStr(string input)
        {
            var values = input.Select(c => Alphabet.IndexOf(c)).ToList();
            
            bool carry = false;
            for (int i = values.Count - 1; i >= 0; i--)
            {
                var newVal = (values[i] + 1) % Alphabet.Length;
                carry = newVal == 0;
                values[i] = newVal;
                if (!carry)
                    break;
            }
            if (carry)
                values.Insert(0, 1);
            return new string(values.Select(i => Alphabet[i]).ToArray());
        }

        private bool CheckRule1(string pwd)
        {
            // increasing straight of at least three letters 
            for (int i = 0; i < (pwd.Length - 2); i++)
            {
                var c0 = pwd[i];
                var c1 = c0 + 1;
                var c2 = c1 + 1;
                
                if (pwd[i + 1] == c1 && pwd[i + 2] == c2)
                    return true;
            }
            return false;
        }
        private static readonly Regex Rule2 = new Regex(@"i|l|o", RegexOptions.Compiled);

        private static readonly Regex Rule3 = new Regex(@".*(\w)\1.*(\w)\2", RegexOptions.Compiled);

        public bool IsPasswordValid(string pwd)
            => CheckRule1(pwd) && !Rule2.IsMatch(pwd) && Rule3.IsMatch(pwd);
        
        public string NextValidPassword(string pwd)
        {
            do
            {
                pwd = IncStr(pwd);
            } while (!IsPasswordValid(pwd));
            return pwd;
        }

        protected override string Part1()
        {
            Assert(IncStr("a"), "b");
            Assert(IncStr("xz"), "ya");
            Assert(!IsPasswordValid("hijklmmn"));
            Assert(!IsPasswordValid("abbceffg"));
            Assert(!IsPasswordValid("abbcegjk"));
            Assert(NextValidPassword("abcdefgh"), "abcdffaa");
            Assert(NextValidPassword("ghijklmn"), "ghjaabcc");

            return NextValidPassword("cqjxjnds");
        }
        protected override string Part2() => NextValidPassword(NextValidPassword("cqjxjnds"));
    }
}
