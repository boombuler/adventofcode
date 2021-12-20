using AdventOfCode.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Validator : ScreenBase
    {
        class Default : OutputMode
        {
            public override void Enter()
            { 
                SetBG(DEFAULT_BACKGROUND);
                SetFG(DEFAULT_FOREGROUND);
            }
        }
        class ValidateOut : OutputMode
        {
            public override void Enter()
            {
                SetBG(DEFAULT_BACKGROUND);
            }

            public void Write(bool ok)
            {
                if (ok)
                {
                    SetFG(0x009900);
                    base.Write("✓");
                }
                else
                {
                    SetFG(COLOR_CRIMSON);
                    base.Write("⨯");
                }
            }
        }

        public void Run(IEnumerable<ISolution> solutions)
        {
            var years = solutions.GroupBy(s => s.Year).OrderBy(g => g.Key);
            foreach(var year in years)
                RunYear(year.Key, year);

            System.Console.ReadLine();
        }

        private void RunYear(int year, IEnumerable<ISolution> solutions)
        {
            Console<Default>().WriteLine($"> {year:d4} < 1111111111222222");
            Console<Default>().WriteLine("1234567890123456789012345");

            int day = 1;

            foreach(var solution in solutions.OrderBy(s => s.Day))
            {
                var offset = solution.Day - day++;
                Console<ValidateOut>().Write(new string(' ', offset));
                Console<ValidateOut>().Write(solution.Validate());
            }
            Console<Default>().WriteLine("");
        }
    }
}
