namespace AdventOfCode;

class SolutionRepository
{
    private static readonly Lazy<List<ISolution>> FSolutions = new(FindSolutions);

    private static List<ISolution> FindSolutions()
        => typeof(Program).Assembly.GetTypes()
            .Where(t => typeof(ISolution).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<ISolution>()
            .ToList();

    public IEnumerable<ISolution> All(int? year, int? day)
        => FSolutions.Value.Where(s => s.Year == (year ?? s.Year) && s.Day == (day ?? s.Day));

    public int? GetMostRecentDayInYear(int? year)
         => year.HasValue ? FSolutions.Value.Where(s => s.Year == year.Value).OrderByDescending(s => s.Day).FirstOrDefault()?.Day : null;

    public int? GetIncompleteOrMostRecentYear()
        => FSolutions.Value.GroupBy(s => s.Year).OrderBy(y => y.Count()).ThenByDescending(y => y.Key).First().Key;

    public ISolution GetSolution(int? year, int? day)
        => FSolutions.Value.FirstOrDefault(s => s.Year == year && s.Day == day);

    public IEnumerable<int> GetSolvedDays(int? year)
        => year.HasValue ? FSolutions.Value.Where(s => s.Year == year.Value).Select(s => s.Day) : Enumerable.Empty<int>();
}
