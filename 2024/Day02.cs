namespace AdventOfCode._2024;

using static Parser;

class Day02 : Solution
{
    private bool IsSave(IEnumerable<int> numbers)
    {
        var distances = numbers.SlidingWindow(2).Select(n => n[0] - n[1]).ToList();
        return (distances.DistinctBy(Math.Sign).Count() == 1) &&
               distances.Select(Math.Abs).All(n => n is 1 or 2 or 3);
    }
    
    private int CountRoutes(string input, Func<List<int>, bool> predicate)
        => input.Lines()
            .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())
            .Count(predicate);
    
    protected override long? Part1()
    {
        int Solve(string input) 
            => CountRoutes(input, IsSave);
        
        Assert(Solve(Sample()), 2);
        return Solve(Input);
    }
    
    protected override long? Part2()
    { 
        int Solve(string input) 
            => CountRoutes(input, line => line.Select((_, i) => line.Where((_, j) => j != i)).Append(line).Any(IsSave));
        
        Assert(Solve(Sample()), 4);
        return Solve(Input);
    }
}
