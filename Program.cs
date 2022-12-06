namespace AdventOfCode;

using System.CommandLine;
using System.CommandLine.Invocation;

class Program
{
    static int Main(string[] args)
    {
        System.Console.OutputEncoding = Encoding.UTF8;
        var command = new RootCommand
            {
                new Option<int?>(new [] {"--year", "-y"}, "Specifies the year for the AoC puzzles"),
                new Option<int?>(new [] {"--day", "-d"}, "Specifies a puzzle day"),

                new Command("validate", "checks if all puzzle solvers work correctly") { Handler = CommandHandler.Create(Validate) },
                new Command("benchmark", "measures the execution time for the given puzzles") { Handler = CommandHandler.Create(Benchmark) },
            };
        command.Handler = CommandHandler.Create(RunPuzzle);
        return command.InvokeAsync(args).Result;
    }

    private static void Benchmark(int? year, int? day)
        => new Benchmarker().Run(new SolutionRepository().All(year, day));

    private static void Validate(int? year, int? day)
        => new Validator().Run(new SolutionRepository().All(year, day));

    private static void RunPuzzle(int? year, int? day)
    {
        var solutions = new SolutionRepository();
        year ??= solutions.GetIncompleteOrMostRecentYear();
        day ??= solutions.GetMostRecentDayInYear(year);

        var target = solutions.GetSolution(year, day);
        if (target == null)
        {
            var eo = new Console.ErrorOut();
            eo.Enter();
            eo.WriteLine($"There is not yet a solution for {year:d4}/{(day ?? 1):d2}");
            eo.Exit();
        }
        else
            new SolutionExecutor().Run(target);
    }
}
