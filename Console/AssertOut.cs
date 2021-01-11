using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Console
{
    class AssertOut : OutputMode
    {
        public override void Enter()
        {
            SetBG(DEFAULT_BACKGROUND);
            base.Enter();
        }
        public void WriteResult(string name, bool result, string errorTxt)
        {
            SetFG(DEFAULT_FOREGROUND);
            base.Write(string.Format("  Assertion {0,10} : ", name));

            if (result)
            {
                SetFG(0x009900);
                base.WriteLine("PASS");
            }
            else
            {
                SetFG(COLOR_CRIMSON);
                base.Write("FAIL");

                if (!string.IsNullOrEmpty(errorTxt))
                {
                    SetFG(0x8B0000);
                    base.Write(" ");
                    base.Write(errorTxt);
                }
                base.WriteLine(null);
            }
        }
    }
}
