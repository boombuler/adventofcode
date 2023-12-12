namespace AdventOfCode._2018;

class Day17 : Solution
{
    enum Orientation { x, y };
    record ClayBlock(int Offset, Orientation Orientation, int RangeFrom, int RangeTo)
    {
        public static readonly Func<string, ClayBlock> Parse = new Regex(@"\w=(?<Offset>\d+), (?<Orientation>\w)=(?<RangeFrom>\d+)\.\.(?<RangeTo>\d+)").ToFactory<ClayBlock>();

        public IEnumerable<Point2D<int>> GetPoints()
        {
            for (int i = RangeFrom; i <= RangeTo; i++)
            {
                if (Orientation == Orientation.x)
                    yield return (i, Offset);
                else
                    yield return (Offset, i);
            }
        }
    }

    enum State { Clay, WaterStanding, WaterFlowing }

    private static long CountWaterTiles(string input, bool includeFlowing)
    {
        var clay = input.Lines().Select(ClayBlock.Parse).SelectMany(c => c.GetPoints()).ToHashSet();

        var flowStates = new Dictionary<Point2D<int>, bool>();
        var (minY, maxY) = clay.MinMax(p => p.Y);
        var (minX, maxX) = clay.MinMax(p => p.X);
        minX--;
        maxX++;

        bool CheckFlowing(Point2D<int> p)
        {
            if (clay.Contains(p))
                return false;
            if (flowStates.TryGetValue(p, out var res))
                return res;
            if (p.Y > maxY)
                return true;

            var flowing = CheckFlowing(p + (0, 1));

            int merge(int dx, Func<int, bool> checkBounds)
            {
                var r = p.X;
                while (checkBounds(r) && !clay.Contains((r + dx, p.Y)))
                {
                    if (CheckFlowing((r, p.Y + 1)))
                    {
                        flowing = true;
                        break;
                    }
                    r += dx;
                }
                return r;
            }

            var left = merge(-1, i => i >= minX);
            var right = merge(+1, i => i <= maxX);

            for (var x = left; x <= right; x++)
                flowStates[(x, p.Y)] = flowing;

            return flowStates[p];
        }

        CheckFlowing((500, 0));

        return flowStates.Where(s => !s.Value || includeFlowing).Count(s => s.Key.Y >= minY);
    }

    protected override long? Part1()
    {
        Assert(CountWaterTiles(Sample(), true), 57);
        return CountWaterTiles(Input, true);
    }

    protected override long? Part2()
    {
        Assert(CountWaterTiles(Sample(), false), 29);
        return CountWaterTiles(Input, false);
    }
}
