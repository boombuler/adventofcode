using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class SolutionResults
    {
        const string RESULT_DIR = "Results";

        private string FileName(ISolution solution)
            => Path.GetFullPath(Path.Combine(RESULT_DIR, solution.Year.ToString(), $"{solution.Day:d2}.txt"));

        public (string Part1, string Part2) GetResults(ISolution solution)
        {
            string path = FileName(solution);
            if (!File.Exists(path))
                return (null, null);

            using (var sr = new StreamReader(path))
                return (sr.ReadLine(), sr.ReadLine());
        }

        public void UpdateResults(ISolution solution, string part1, string part2)
        {
            if (!Directory.Exists("Results"))
                return;

            var (old1, old2) = GetResults(solution);
            if (old1 == part1 && old2 == part2)
                return;

            var path = FileName(solution);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, part1 + "\n" + part2);
        }
    }
}
