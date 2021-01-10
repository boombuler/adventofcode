using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Console
{
    class AssertOut : OutputMode
    {
        public void WriteResult(string name, bool result, string errorTxt)
        {
            base.Write(string.Format("  Assertion {0,10} : ", name));
            var col = Foreground;

            if (result)
            {
                Foreground = ConsoleColor.Green;
                base.WriteLine("PASS");
            }
            else
            {
                Foreground = ConsoleColor.Red;
                base.Write("FAIL");

                if (!string.IsNullOrEmpty(errorTxt))
                {
                    Foreground = ConsoleColor.DarkRed;
                    base.Write(" ");
                    base.Write(errorTxt);
                }
                base.WriteLine(null);
            }
            Foreground = col;
        }
    }
}
