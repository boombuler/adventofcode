using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Console
{
    class DebugOut : OutputMode
    {
        private ConsoleColor fCC;

        public override void Enter()
        {
            fCC = Foreground;
            Foreground = ConsoleColor.Blue;
            base.Enter();
        }

        public override void Exit()
        {
            Foreground = fCC;
            base.Exit();
        }

        public override void Write(string content)
            => base.Write("        " + content);
    }
}
