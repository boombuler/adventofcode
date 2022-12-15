namespace AdventOfCode._2015;

class Day02 : Solution
{
    class Package
    {
        public int Width { get; }
        public int Length { get; }
        public int Height { get; }

        private int SideA => Length * Width;
        private int SideB => Length * Height;
        private int SideC => Width * Height;

        public Package(string size)
        {
            var sizes = size.Split('x');
            Width = int.Parse(sizes[0]);
            Length = int.Parse(sizes[1]);
            Height = int.Parse(sizes[2]);
        }

        public long GetPaperSize()
        {
            var sides = new[] { SideA, SideB, SideC };
            Array.Sort(sides);
            return sides.Select(s => s * 2).Sum() + sides[0];
        }

        public long GetRibbonLength() =>
            new[] { Width, Length, Height }.Order().Take(2).Select(s => s * 2).Sum()
            + (Width * Length * Height);

    }

    private long GetTotal(Func<Package, long> selector)
        => Input.Lines().Select(l => selector(new Package(l))).Sum();
    protected override long? Part1()
    {
        Assert(new Package("2x3x4").GetPaperSize(), 58);
        Assert(new Package("1x1x10").GetPaperSize(), 43);
        return GetTotal(p => p.GetPaperSize());
    }
    protected override long? Part2()
    {
        Assert(new Package("2x3x4").GetRibbonLength(), 34);
        Assert(new Package("1x1x10").GetRibbonLength(), 14);
        return GetTotal(p => p.GetRibbonLength());
    }
}
