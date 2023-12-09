namespace AdventOfCode._2022;

class Day07 : Solution
{
    private static IEnumerable<long> GetDirSizes(string input)
    {
        var curPath = ImmutableStack<string>.Empty;
        var sizes = new Dictionary<ImmutableStack<string>, long>();

        foreach(var line in input.Lines())
        {
            switch (line.Split(' '))
            {
                case ["$", "cd", ".."]:
                    curPath = curPath.Pop(); break;
                case ["$", "cd", var dir]:
                    curPath = curPath.Push(dir); break;
                case [var size, _] when long.TryParse(size, out var fileSize):
                    for (var p = curPath; !p.IsEmpty; p = p.Pop())
                        sizes[p] = sizes.GetValueOrDefault(p) + fileSize;
                    break;
            }
        }

        return sizes.Values;
    }
    
    private static long GetSumOfSmallDirs(string input) 
        => GetDirSizes(input).Where(d => d < 100000).Sum();

    private static long SizeOfDirToDelete(string input)
    {
        var sizes = GetDirSizes(input);
        var requiredSize = sizes.Max() - 40000000;
        return sizes.Where(s => s >= requiredSize).Min();
    }

    protected override long? Part1()
    {
        Assert(GetSumOfSmallDirs(Sample()), 95437);
        return GetSumOfSmallDirs(Input);
    }

    protected override long? Part2()
    {
        Assert(SizeOfDirToDelete(Sample()), 24933642);
        return  SizeOfDirToDelete(Input);
    }
}
