using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Console
{
    abstract class OutputMode
    {
        private bool fHasLB = false;
        protected ConsoleColor Foreground
        {
            get => System.Console.ForegroundColor;
            set => System.Console.ForegroundColor = value;
        }
        public virtual void Enter()
        {
            System.Console.WriteLine();
        }

        public virtual void Exit()
        {
            if (!fHasLB)
                System.Console.WriteLine();
        }

        public virtual void Write(string content)
        {
            System.Console.Write(content);
            fHasLB = content.EndsWith(Environment.NewLine);
                 
        }
        public virtual void WriteLine(string content)
        {
            System.Console.WriteLine(content);
            fHasLB = true;
        }
    }
}
