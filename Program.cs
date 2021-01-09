using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        private static IEnumerable<ISolution> FindSolutions()
            => typeof(Program).Assembly.GetTypes()
                .Where(t => typeof(ISolution).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<ISolution>();

        static int Main(string[] args)
        {
            var command = new RootCommand
            {
                new Option<int?>(new [] {"--year", "-y"}, "Specifies the year for the AoC puzzles"),
                new Option<int?>(new [] {"--day", "-d"}, "Specifies a puzzle day"),
            };
            command.Handler = CommandHandler.Create((int? year, int? day) =>
            {
                var solutions = FindSolutions().ToList();
                if (!year.HasValue)
                    year = solutions.GroupBy(s => s.Year).OrderBy(y => y.Count()).ThenByDescending(y => y.Key).First().Key;
                if (!day.HasValue)
                    day = solutions.Where(s => s.Year == year).OrderByDescending(s => s.Day).FirstOrDefault()?.Day;

                var target = solutions.FirstOrDefault(s => s.Year == year && s.Day == day);
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
