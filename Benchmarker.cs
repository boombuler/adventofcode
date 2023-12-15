namespace AdventOfCode;

using System.Diagnostics;
using AdventOfCode.Console;

class Benchmarker : ScreenBase
{
    private void WriteLine(string s)
        => Console<DefaultOut>().WriteLine(s);

    private static async Task<TimeSpan> Measure(Func<Task<string>> actn)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await actn();
        }
        finally
        {
            sw.Stop();
        }
        return sw.Elapsed;
    }

    public async Task Run(IEnumerable<ISolution> solutions)
    {
        solutions = solutions.OrderBy(s => s.Year).ThenBy(s => s.Day);
        if (!solutions.Any())
            return;

        var total = new TimeSpan();
        var yearTotal = new TimeSpan();
        int year = 0;
        void WriteFooter()
        {
            WriteLine($"├─────┴───────────┴───────────┤");
            WriteLine($"│            Total: {yearTotal:mm\\:ss\\.fff} │");
            WriteLine($"└─────────────────────────────┘");
            WriteLine("");
        }

        foreach (var solution in solutions)
        {
            if (solution.Year != year)
            {
                if (year != 0)
                    WriteFooter();
                year = solution.Year;
                yearTotal = new TimeSpan();
                WriteLine($"┌─────────────────────────────┐");
                WriteLine($"│             {year}            │");
                WriteLine($"├─────┬───────────┬───────────┤");
                WriteLine($"│ Day │ Part 1    │ Part 2    │");
                WriteLine($"├─────┼───────────┼───────────┤");
            }

            var sb = new StringBuilder();
            var e1 = await Measure(() => solution.Part1(null));
            total += e1;
            yearTotal += e1;
            sb.Append("│  ")
                .AppendFormat("{0,2}", solution.Day)
                .Append(" │ ")
                .Append(e1.ToString("mm\\:ss\\.fff"))
                .Append(" │ ");
            if (solution.Day != 25)
            {
                var e2 = await Measure(() => solution.Part2(null));
                total += e2;
                yearTotal += e2;
                sb.Append(e2.ToString("mm\\:ss\\.fff"));
            }
            else
                sb.Append("---      ");
            sb.Append(" │");
            WriteLine(sb.ToString());
        }

        WriteFooter();

        WriteLine("");
        WriteLine($"        Total Time: {total:mm\\:ss\\.fff}");
    }
}
