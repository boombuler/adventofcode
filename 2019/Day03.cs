namespace AdventOfCode._2019;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Utils;

class Day03 : Solution
{
    private IEnumerable<Point2D> TracePath(string path)
    {
        var cur = Point2D.Origin;
        (char direction, int amount) = ('\0', 0);

        using var sr = new StringReader(path + ",");
        while (sr.TryRead(out char c))
        {
            if (c == ',')
            {
                var off = direction switch
                {
                    'U' => (0, -1),
                    'D' => (0, 1),
                    'L' => (-1, 0),
                    _ => (1, 0)
                };
                for (int i = 0; i < amount; i++)
                    yield return cur += off;
                (direction, amount) = ('\0', 0);
            }
            else if (c is >= '0' and <= '9')
                amount = (amount * 10) + (c - '0');
            else
                direction = c;
        }
    }

    private long ClosestIntersection(params string[] paths)
        => paths.Select(TracePath).Aggregate(Enumerable.Intersect).Min(Point2D.Origin.ManhattanDistance);

    protected override long? Part1()
    {
        Assert(ClosestIntersection("R8,U5,L5,D3", "U7,R6,D4,L4"), 6);
        Assert(ClosestIntersection("R75,D30,R83,U83,L12,D49,R71,U7,L72", "U62,R66,U55,R34,D71,R55,D58,R83"), 159);
        Assert(ClosestIntersection("R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51", "U98,R91,D20,R16,D67,R40,U7,R15,U6,R7"), 135);

        return ClosestIntersection(Input.Lines().ToArray());
    }

    private long FirstIntersection(params string[] paths)
    {
        var lists = paths.Select(TracePath).Select(Enumerable.ToList).ToList();
        return lists.Aggregate<IEnumerable<Point2D>>(Enumerable.Intersect).Select(pt => lists.Count + lists.Sum(l => l.IndexOf(pt))).Min();
    }

    protected override long? Part2()
    {
        Assert(FirstIntersection("R8,U5,L5,D3", "U7,R6,D4,L4"), 30);
        Assert(FirstIntersection("R75,D30,R83,U83,L12,D49,R71,U7,L72", "U62,R66,U55,R34,D71,R55,D58,R83"), 610);
        Assert(FirstIntersection("R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51", "U98,R91,D20,R16,D67,R40,U7,R15,U6,R7"), 410);

        return FirstIntersection(Input.Lines().ToArray());
    }
}
