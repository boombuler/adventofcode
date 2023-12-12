namespace AdventOfCode._2021;

using Point = Point2D<int>;
class Day25 : Solution
{
    record Map(int Width, int Height)
    {
        public (Dictionary<Point, char> NewState, int Moved) Move(Dictionary<Point, char> state, char c, Point direction)
        {
            var result = new Dictionary<Point, char>(state);
            var moved = 0;
            foreach (var (k, v) in state)
            {
                if (v != c)
                    continue;
                var newPos = k + direction;
                newPos = (newPos.X % Width, newPos.Y % Height);
                if (state.ContainsKey(newPos))
                    continue;

                result.Remove(k);
                result[newPos] = v;
                moved++;
            }
            return (result, moved);
        }

    }

    private static int Solve(string input)
    {
        var cucumbers = input.Cells(c => c, c => c != '.');
        var map = new Map(cucumbers.Keys.Max(c => c.X) + 1, cucumbers.Keys.Max(c => c.Y) + 1);
        var turn = 0;

        while (true)
        {
            turn++;
            var (state1, movedEast) = map.Move(cucumbers, '>', Point.Right);
            var (state2, movedSouth) = map.Move(state1, 'v', Point.Down);
            if (movedEast + movedSouth == 0)
                break;
            cucumbers = state2;
        }
        return turn;
    }

    protected override long? Part1()
    {
        Assert(Solve(Sample()), 58);
        return Solve(Input);
    }
}
