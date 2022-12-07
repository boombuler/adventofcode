namespace AdventOfCode._2022;

class Day07 : Solution
{
    private Dictionary<string, long> GetDirSizes(string input)
    {
        var curPath = ImmutableStack<string>.Empty;
        var sizes = new Dictionary<ImmutableStack<string>, long>();
        var console = new StringReader(input);

        while (console.TryReadLine(out var cmd)) 
        {
            if (cmd == "$ cd ..")
                curPath = curPath.Pop();
            else if (cmd.StartsWith("$ cd "))
                curPath = curPath.Push(cmd.Substring(5));
            else if (cmd == "$ ls")
            {
                while (console.Peek() is not ((< 0) or '$'))
                {
                    if (!long.TryParse(console.ReadLine().Split(' ')[0], out long fileSize))
                        fileSize += 0;

                    for(var p = curPath; !p.IsEmpty; p = p.Pop())
                        sizes[p] = sizes.GetValueOrDefault(p, 0) + fileSize;
                }
            }
        }

        return sizes.ToDictionary(
            kvp => string.Join("/", kvp.Key.Reverse()), 
            kvp => kvp.Value);
    }
    
    private long GetSumOfSmallDirs(string input) 
        => GetDirSizes(input).Values.Where(d => d < 100000).Sum();

    private long SizeOfDirToDelete(string input)
    {
        var sizes = GetDirSizes(input);
        var requiredSize = sizes["/"] - 40000000;
        return sizes.Values.OrderBy(Functional.Identity).First(s => s >= requiredSize);
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
