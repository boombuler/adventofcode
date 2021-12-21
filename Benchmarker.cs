using AdventOfCode.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class Benchmarker : ScreenBase
    {
        private void WriteLine(string s)
            => Console<DefaultOut>().WriteLine(s);

        private TimeSpan Measure(Action actn)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                actn();
            }
            finally
            {
                sw.Stop();
            }
            return sw.Elapsed;
        }

        public void Run(IEnumerable<ISolution> solutions)
        {
            solutions = solutions.OrderBy(s => s.Year).ThenBy(s => s.Day);
            if (!solutions.Any())
                return;

            var total = new TimeSpan();
            int year = 0;
            foreach (var solution in solutions)
            {
                if (solution.Year != year)
                {
                    if (year != 0)
                    {
                        WriteLine($"└─────┴───────────┴───────────┘");
                        WriteLine("");
                    }
                    year = solution.Year;
                    WriteLine($"┌─────────────────────────────┐");
                    WriteLine($"│             {year}            │");
                    WriteLine($"├─────┬───────────┬───────────┤");
                    WriteLine($"│ Day │ Part 1    │ Part 2    │");
                    WriteLine($"├─────┼───────────┼───────────┤");
                }

                var sb = new StringBuilder();
                var e1 = Measure(() => solution.Part1(null));
                total += e1;
                sb.Append("│  ")
                    .AppendFormat("{0,2}", solution.Day)
                    .Append(" │ ")
                    .Append(e1.ToString("mm\\:ss\\.fff"))
                    .Append(" │ ");
                if (solution.Day != 25)
                {
                    var e2 = Measure(() => solution.Part2(null));
                    total += e2;
                    sb.Append(e2.ToString("mm\\:ss\\.fff"));
                }
                else
                    sb.Append("---      ");
                sb.Append(" │");
                WriteLine(sb.ToString());
            }

            WriteLine($"└─────┴───────────┴───────────┘");

            WriteLine("");
            WriteLine($"        Total Time: {total:mm\\:ss\\.fff}");
        }
    }
}
