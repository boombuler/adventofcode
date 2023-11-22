namespace AdventOfCode._2021;

internal class Day22 : Solution
{
    record Region(Point3D Min, Point3D Max)
    {
        public long Size => (Max.X - Min.X + 1) * (Max.Y - Min.Y + 1) * (Max.Z - Min.Z + 1);

        public Region Intersect(Region other)
        {
            if (Min.X > other.Max.X || Max.X < other.Min.X ||
                Min.Y > other.Max.Y || Max.Y < other.Min.Y ||
                Min.Z > other.Max.Z || Max.Z < other.Min.Z)
                return null;

            return new Region(
                (Math.Max(Min.X, other.Min.X), Math.Max(Min.Y, other.Min.Y), Math.Max(Min.Z, other.Min.Z)),
                (Math.Min(Max.X, other.Max.X), Math.Min(Max.Y, other.Max.Y), Math.Min(Max.Z, other.Max.Z))
            );
        }
    }

    record Instruction(Region Region, bool On);

    private Instruction ParseInstruction(string instr)
    {
        var (onStr, (dirStr, _)) = instr.Split(' ');
        var (x, (y, (z, _))) = dirStr.Split(',').Select(s => s[2..].Split("..").Select(long.Parse).ToArray());
        return new Instruction(new Region((x[0], y[0], z[0]), (x[1], y[1], z[1])), onStr == "on");
    }

    private long ToggleCubes(string input, bool WithBB = true)
    {
        var bbox = new Region((-50, -50, -50), (50, 50, 50));
        var instrs = input.Lines().Select(ParseInstruction);

        if (WithBB)
            instrs = instrs.Select(i => i with { Region = bbox.Intersect(i.Region) }).Where(i => i.Region != null);

        var instructions = new List<Instruction>();

        foreach (var instr in instrs)
        {
            var newInstructions = from i in instructions
                                  let r = i.Region.Intersect(instr.Region)
                                  where r != null
                                  select new Instruction(r, !i.On);
            if (instr.On)
                newInstructions = newInstructions.Append(instr);
            instructions.AddRange(newInstructions.ToList());
        }

        return instructions.Sum(i => (i.On ? 1 : -1) * i.Region.Size);
    }

    protected override long? Part1()
    {
        Assert(ToggleCubes(Sample("1")), 39);
        Assert(ToggleCubes(Sample("2")), 590784);
        return ToggleCubes(Input);
    }

    protected override long? Part2() => ToggleCubes(Input, false);
}
