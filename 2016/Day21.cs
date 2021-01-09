using AdventOfCode.Utils;
using System;
using System.Linq;

namespace AdventOfCode._2016
{
    class Day21 : Solution<string>
    {
        private string Scramble(string password, string rules, bool reverse = false)
        {
            char[] pwd = password.ToCharArray();
            var commands = rules.Lines();
            if (reverse)
                commands = commands.Reverse();
            foreach (var command in commands)
            {
                var parts = command.Split(' ');
                switch ((parts[0], parts[1]))
                {
                    case ("swap", "position"):
                        {
                            // swap position X with position Y
                            (int x, int y) = (int.Parse(parts[2]), int.Parse(parts[5]));
                            (pwd[x], pwd[y]) = (pwd[y], pwd[x]);
                        }
                        break;
                    case ("swap", "letter"):
                        {
                            // swap letter X with letter Y
                            (int x, int y) = (Array.IndexOf(pwd, parts[2][0]), Array.IndexOf(pwd, parts[5][0]));
                            (pwd[x], pwd[y]) = (pwd[y], pwd[x]);
                        }
                        break;
                    case ("rotate", "left"):
                        // rotate left X steps
                        pwd.RotateRight((reverse ? 1 : -1) * int.Parse(parts[2]));
                        break;
                    case ("rotate", "right"):
                        // rotate right X steps
                        pwd.RotateRight((reverse ? -1 : 1) * int.Parse(parts[2]));
                        break;
                    case ("rotate", "based"):
                        {
                            // rotate based on position of letter X
                            int index = Array.IndexOf(pwd, parts[6][0]);
                            if (reverse)
                            {
                                for(int oldIdx = pwd.Length-1; oldIdx >= 0 ; oldIdx--)
                                {
                                    int shift = 1 + oldIdx + (oldIdx >= 4 ? 1 : 0);
                                    int newIdx = (oldIdx + shift) % pwd.Length;
                                    if (newIdx == index)
                                    {
                                        pwd.RotateRight(-shift);
                                        break;
                                    }
                                }
                            }
                            else
                                pwd.RotateRight(1 + index + (index >= 4 ? 1 : 0));
                        }
                        break;
                    case ("reverse", "positions"):
                        {
                            // reverse positions X through Y
                            (int x, int y) = (int.Parse(parts[2]), int.Parse(parts[4]));
                            foreach (var c in pwd.Skip(x).Take(1 + y - x).Reverse())
                                pwd[x++] = c;
                        }
                        break;
                    case ("move", "position"):
                        {
                            // move position X to position Y
                            (int x, int y) = (int.Parse(parts[2]), int.Parse(parts[5]));
                            if (reverse)
                                (x, y) = (y, x);
                            char c = pwd[x];
                            if (x < pwd.Length - 1) 
                                Array.Copy(pwd, x + 1, pwd, x, pwd.Length - 1 - x);
                            Array.Copy(pwd, y, pwd, y + 1, pwd.Length - 1 - y);
                            pwd[y] = c;
                        }
                        break;
                }
            }
            return new string(pwd);
        }

        protected override string Part1()
        {
            Assert(Scramble("abcde", Sample()), "decab");
            return Scramble("abcdefgh", Input);
        }

        protected override string Part2()
        {
            Assert(Scramble("decab", Sample(), true), "abcde");
            Assert(Scramble("gfdhebac", Input, true), "abcdefgh");
            return Scramble("fbgdceah", Input, true);
        }
    }
}
