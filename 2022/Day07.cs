namespace AdventOfCode._2022;

class Day07 : Solution
{
    private IEnumerable<long> GetDirSizes(string input)
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
    
    private long GetSumOfSmallDirs(string input) 
        => GetDirSizes(input).Where(d => d < 100000).Sum();

    private long SizeOfDirToDelete(string input)
    {
        var sizes = GetDirSizes(input);
        var requiredSize = sizes.Max() - 40000000;
        return sizes.Where(s => s >= requiredSize).Min();
    }

    protected override long? Part1()
    {
        Assert(95437, GetSumOfSmallDirs(Sample()));
        return GetSumOfSmallDirs(Input);
    }

    protected override long? Part2()
    {
        Assert(24933642, SizeOfDirToDelete(Sample()));
        return  SizeOfDirToDelete(Input);
    }
}
