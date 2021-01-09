using System;
using System.Collections.Generic;
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

        private static ISolution GuessTarget(IEnumerable<ISolution> solutions)
        {
            var years = solutions.GroupBy(s => s.Year);
            // Get the first incomplete year, or the latest.
            var year = years.OrderBy(y => y.Count()).ThenByDescending(y => y.Key).First();
            // And return the last implemented type for that year.
            return year.OrderByDescending(s => s.Day).First(); 
        }


        static void Main(string[] args)
        {
            GuessTarget(FindSolutions()).Run();
        }
    }
}
