namespace AdventOfCode._2024;

class Day09 : Solution
{
    private readonly string Sample = "2333133121414131402";

    class File(long Id)
    {
        public long Index { get; set; }
        public long Size { get; set; }

        public long Checksum() 
        {
            long end = Index + Size;
            return Id * ((end * (end - 1)) - (Index * (Index - 1))) / 2;
        }
    }
    
    delegate IEnumerable<File> CreateFiles(int id, int idx, int size);

    private (List<File> Files, LinkedList<(long Index, long Size)> Space) LoadDiskStructure(string input, CreateFiles createFiles)
    {
        if (input.Length % 2 == 1)
            input += "0";
        var files = new List<File>();
        var unallocated = new List<(long Index, long Size)>();

        int idx = 0;
        for (int i = 0; i < input.Length; i += 2)
        {
            int fileSize = input[i] - '0';
            files.AddRange(createFiles(i / 2, idx, fileSize));
            idx += fileSize;

            if (input[i + 1] - '0' is int spaceSize and > 0)
            {
                unallocated.Add((idx, spaceSize));
                idx += spaceSize;
            }
        }
        return (files, new LinkedList<(long Index, long Size)>(unallocated));
    }

    private long Defrag(string input, CreateFiles createFiles)
    {
        var (files, unallocated) = LoadDiskStructure(input, createFiles);

        for (int i = files.Count-1; i >= 0; i--)
        {
            var file = files[i];
            for (var node = unallocated.First; node != null; node = node.Next)
            {
                if (node.Value.Index > file.Index)
                    break;
                if (node.Value.Size < file.Size)
                    continue;
                file.Index = node.Value.Index;
                if (file.Size == node.Value.Size)
                    unallocated.Remove(node);
                else
                    node.Value = (node.Value.Index + file.Size, node.Value.Size - file.Size);
            }
        }
        return files.Sum(f => f.Checksum());
    }

    protected override long? Part1()
    {
        long Solve(string input) 
            => Defrag(input, (id, idx, size) => Enumerable.Range(0, size).Select((s,i) => new File(id) { Index = i+idx, Size = 1}));

        Assert(Solve(Sample), 1928);
        return Solve(Input);
    }
    protected override long? Part2()
    {
        long Solve(string input)
            => Defrag(input, (id, idx, size) => [new File(id) { Index = idx, Size = size }]);

        Assert(Solve(Sample), 2858);
        return Solve(Input);
    }
}
