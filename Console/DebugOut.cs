using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Console
{
    class DebugOut : OutputMode
    {
        public override void Enter()
        {
            SetBG(DEFAULT_BACKGROUND);
            SetFG(0x6495ED);
            base.Enter();
        }

        public override void Write(string content)
            => base.Write("        " + content);
    }
}
