using AdventHelper;
using System;

namespace _2016_02
{
    class Program : ProgramBase<string>
    {
        static void Main(string[] args) => new Program().Run();

        private string ReadInstructions(string file, string[] keypad)
        {
            string code = string.Empty;
            int row = 1, col = 1;

            for (int r = 0; r < keypad.Length; r++)
            {
                for (int c = 0; c < keypad[r].Length; c++)
                {
                    if (keypad[r][c] == '5')
                    {
                        row = r;
                        col = c;
                    }
                }
            }
            
            foreach(var line in ReadLines(file))
            {
                foreach(var c in line)
                {
                    int nr = row, nc = col;
                    switch(c)
                    {
                        case 'U': nr--; break;
                        case 'D': nr++; break;
                        case 'L': nc--; break;
                        case 'R': nc++; break;
                    }
                    
                    if (keypad[nr][nc] != ' ')
                    {
                        row = nr;
                        col = nc;
                    }
                }

                code += keypad[row][col];
            }
            return code;
        }

        protected override string Part1()
        {
            var kp = new[] {
                "     ",
                " 123 ",
                " 456 ",
                " 789 ",
                "     "
            };
            Assert(ReadInstructions("Sample", kp), "1985");
            return ReadInstructions("Input", kp);
        }

        protected override string Part2()
        {
            var kp = new[]
            {
               "       ",
               "   1   ",
               "  234  ",
               " 56789 ",
               "  ABC  ",
               "   D   ",
               "       ",
            };
            Assert(ReadInstructions("Sample", kp), "5DB3");
            return ReadInstructions("Input", kp);
        }
    }
}
