using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class SolutionRepository
    {
        private static Lazy<List<ISolution>> fSolutions = new Lazy<List<ISolution>>(FindSolutions);

        private static List<ISolution> FindSolutions()
            => typeof(Program).Assembly.GetTypes()
                .Where(t => typeof(ISolution).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<ISolution>()
                .ToList();


        public int? GetMostRecentDayInYear(int? year)
             => year.HasValue ? fSolutions.Value.Where(s => s.Year == year.Value).OrderByDescending(s => s.Day).FirstOrDefault()?.Day : null;

        public int? GetIncompleteOrMostRecentYear() 
            =>fSolutions.Value.GroupBy(s => s.Year).OrderBy(y => y.Count()).ThenByDescending(y => y.Key).First().Key;

        public ISolution GetSolution(int? year, int? day)
            => fSolutions.Value.FirstOrDefault(s => s.Year == year && s.Day == day);

        public IEnumerable<int> GetSolvedDays(int? year)
            => year.HasValue ? fSolutions.Value.Where(s => s.Year == year.Value).Select(s => s.Day) : Enumerable.Empty<int>();
    }
}
