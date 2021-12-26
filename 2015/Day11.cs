namespace AdventOfCode._2015;

using System.Linq;
using System.Text.RegularExpressions;

class Day11 : Solution<string>
{
    private static string IncStr(string input)
    {
        var chars = input.ToArray();
        for (int i = chars.Length - 1; i >= 0; i--)
        {
            var a = (char)(chars[i] + 1);
            if (a > 'z')
                a = 'a';
            chars[i] = a;
            if (a != 'a')
            {
                EnsureRule2(chars, i);
                break;
            }
        }
        return new string(chars);
    }

    private static void EnsureRule2(char[] chars, int startIdx = 0)
    {
        for (int i = startIdx; i < chars.Length; i++)
        {
            var a = chars[i];
            if (a is 'i' or 'l' or 'o')
            {
                a++;
                chars[i] = a;
                for (int ii = i + 1; ii < chars.Length; ii++)
                    chars[ii] = 'a';
                return;
            }
        }
    }

    private static bool CheckRule1(string pwd)
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

    private static readonly Regex Rule3 = new(@".*(\w)\1.*(\w)\2", RegexOptions.Compiled);

    public static bool IsPasswordValid(string pwd)
        => CheckRule1(pwd) && Rule3.IsMatch(pwd);

    public static string NextValidPassword(string pwd)
    {
        var pwdArr = pwd.ToArray();
        EnsureRule2(pwdArr);
        pwd = new string(pwdArr);

        do
        {
            pwd = IncStr(pwd);
        } while (!IsPasswordValid(pwd));
        return pwd;
    }

    protected override string Part1()
    {
        Assert(!IsPasswordValid("hijklmmn"));
        Assert(!IsPasswordValid("abbceffg"));
        Assert(!IsPasswordValid("abbcegjk"));
        Assert(NextValidPassword("abcdefgh"), "abcdffaa");
        Assert(NextValidPassword("ghijklmn"), "ghjaabcc");

        return NextValidPassword(Input);
    }
    protected override string Part2() => NextValidPassword(NextValidPassword(Input));
}
