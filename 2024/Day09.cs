namespace AdventOfCode._2024;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Day09 : Solution
{
    private readonly string Sample = "2333133121414131402";

    class File(long Id)
    {
        public int Idx { get; set; }
        public int Size { get; set; }

        public long Sum()
        {
            long sum = 0;
            for (int i = 0; i < Size; i++)
            {
                sum += (long)Id * (long)(Idx + i);
            }
            return sum;
        }
    }


    private long Solve(string input, bool splitFile = true)
    {
        if (input.Length % 2 == 1)
            input += "0";
        var files = new LinkedList<File>();
        var space = new LinkedList<File>();

        int idx = 0;
        for (int i = 0; i < input.Length; i += 2)
        {
            int fileSize = input[i] - '0';
            int spaceSize = input[i + 1] - '0';
            if (splitFile)
            {
                for (int j = 0; j < fileSize; j++)
                    files.AddLast(new File(i/2) { Idx = idx++, Size = 1 });
            }
            else
            {
                files.AddLast(new File(i / 2) { Idx = idx, Size = fileSize });
                idx += fileSize;
            }
            space.AddLast(new File(i / 2) { Idx = idx, Size = spaceSize });
            idx += spaceSize;
        }


        for (var fileToMove = files.Last; fileToMove != null; fileToMove = fileToMove.Previous)
        {
            for (var s = space.First; s != null; s = s.Next)
            {
                if (s.Value.Idx > fileToMove.Value.Idx)
                    break;
                var newSize = s.Value.Size - fileToMove.Value.Size;
                if (newSize >= 0)
                {
                    fileToMove.Value.Idx = s.Value.Idx;
                    if (newSize > 0)
                    {
                        s.Value.Idx += fileToMove.Value.Size;
                        s.Value.Size = newSize;
                    }
                    else
                    {
                        space.Remove(s);
                    }
                    break;
                }
            }
        }
        return files.Sum(f => f.Sum());

    }

    protected override long? Part1()
    {
        Assert(Solve(Sample), 1928);
        return Solve(Input);
    }
    protected override long? Part2()
    {
        Assert(Solve(Sample, splitFile: false), 2858);
        return Solve(Input, splitFile: false);
    }
}
