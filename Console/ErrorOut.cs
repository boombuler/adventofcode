using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventOfCode.Console
{
    class ErrorOut : OutputMode
    {
        int BoxWidth => Math.Min(System.Console.BufferWidth, 80);

        public override void Enter()
        {
            base.Enter();
            SetBG(DEFAULT_BACKGROUND);
            SetFG(COLOR_CRIMSON);
            var hLine = new string('═', BoxWidth - 3);
            base.WriteLine(" ╔" + hLine + "╗");
        }

        public override void Exit()
        {
            var hLine = new string('═', BoxWidth - 3);
            base.Write(" ╚" + hLine + "╝");
            base.Exit();
        }

        private void WriteTextLine(string line)
        {
            var words = line.Split(' ');

            var maxLineLen = BoxWidth - 5;

            var sb = new StringBuilder();

            void WriteOutput()
            {
                if (sb.Length == 0)
                    return;

                sb.Append(new string(' ', maxLineLen - sb.Length));

                base.Write(" ║ ");
                base.Write(sb.ToString());
                base.WriteLine(" ║");

                sb.Clear();
            }

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];

                if (sb.Length + word.Length + 1 > maxLineLen)
                    WriteOutput();
                else if (sb.Length > 0)
                    sb.Append(' ');
                
                sb.Append(word);
            }
            WriteOutput();
        }

        public override void WriteLine(string content)
        {
            Write(content);
            Write(Environment.NewLine);
        }

        public override void Write(string content)
        {
            using (var sr = new StringReader(content))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null)
                        return;
                    WriteTextLine(line);
                }
            }
        }
    }
}
