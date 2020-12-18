using System;
using System.Collections.Generic;
using System.Text;

namespace AdventHelper.Console
{
    class AssertOut : OutputMode
    {
        public void WriteResult(string name, bool result)
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
                base.WriteLine("FAIL");
            }
            Foreground = col;
        }
    }
}
