using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace AdventOfCode
{
    class Program
    {
        static int Main(string[] args)
        {
            var command = new RootCommand
            {
                new Option<int?>(new [] {"--year", "-y"}, "Specifies the year for the AoC puzzles"),
                new Option<int?>(new [] {"--day", "-d"}, "Specifies a puzzle day"),
            };
            command.Handler = CommandHandler.Create((int? year, int? day) =>
            {
                var solutions = new SolutionRepository();
                year ??= solutions.GetIncompleteOrMostRecentYear();
                day ??= solutions.GetMostRecentDayInYear(year);

                var target = solutions.GetSolution(year, day);
                if (target == null)
                {
                    var eo = new Console.ErrorOut();
                    eo.Enter();
                    eo.WriteLine($"There is not yet a solution for {year:d4}/{(day??1):d2}");
                    eo.Exit();
                }
                else
                    target.Run();
            });
            return command.InvokeAsync(args).Result;
        }
    }
}
