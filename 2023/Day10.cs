namespace AdventOfCode._2023;

using System.Buffers;

using static Point2D<int>;

class Day10 : Solution
{
    private static readonly FrozenDictionary<(char Char, Point2D<int> Dir), Point2D<int>> Connections = new Dictionary<(char, Point2D<int>), Point2D<int>>()
    {
        [('|', Up)] = Up,
        [('|', Down)] = Down,
        [('-', Left)] = Left,
        [('-', Right)] = Right,
        [('L', Left)] = Up,
        [('L', Down)] = Right,
        [('J', Right)] = Up,
        [('J', Down)] = Left,
        [('7', Right)] = Down,
        [('7', Up)] = Left,
        [('F', Left)] = Down,
        [('F', Up)] = Right,
    }.ToFrozenDictionary();

    private static (HashSet<Point2D<int>> Loop, StringMap<char> Map) ParseMap(string input)
    {
        var map = input.AsMap();
        var pos = map.Find('S') ?? throw new InvalidInputException();
        var possibleStartDirections = (
            from d in new Point2D<int>[] { Up, Down, Left, Right }
            let n = map.GetValueOrDefault(pos + d, '.')
            where Connections.ContainsKey((n, d))
            select d
        ).ToList();
        map[pos] = Connections.Keys.Select(k => k.Char).Distinct()
            .Single(c => possibleStartDirections.All(d => Connections.ContainsKey((c, -d))));

        var dir = possibleStartDirections.First();
        var loop = new HashSet<Point2D<int>>();
        while (loop.Add(pos))
        {
            pos += dir ?? Origin;
            dir = Connections.GetValueOrDefault((map[pos], dir ?? Origin));
        }
        return (loop, map);
    }

    private static long CountEnclosedTiles(string input)
    {
        var (loop, map) = ParseMap(input);
        foreach (var idx in map.Select(m => m.Index).Where(m => !loop.Contains(m)))
            map[idx] = '.'; // remove scrap tiles

        long sum = 0;
        var vertical = SearchValues.Create(['|', 'L', 'J']);
        foreach (var row in map.Rows())
        {
            var inside = false;
            foreach (var cell in row)
            {
                if (vertical.Contains(cell))
                    inside = !inside;
                else if (inside && cell == '.')
                    sum++;
            }
        }
        return sum;
    }

    protected override long? Part1()
    {
        static long Solve(string input)
            => ParseMap(input).Loop.Count / 2;

        Assert(Solve(Sample("1")), 8);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        Assert(CountEnclosedTiles(Sample("2")), 4);
        Assert(CountEnclosedTiles(Sample("3")), 8);
        Assert(CountEnclosedTiles(Sample("4")), 10);

        return CountEnclosedTiles(Input);
    }
}
