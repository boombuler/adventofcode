namespace AdventOfCode._2019;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Utils;

class Day18 : Solution
{
    const char DOOR = '@';
    const char WALL = '#';
    const char FLOOR = '.';

    class Map
    {
        private readonly Dictionary<Point2D, char> fWalkableTiles;
        private readonly Dictionary<char, Point2D> fPOI;
        private readonly Dictionary<(char, char), (long, string)?> fPaths = new();
        public IEnumerable<char> Keys => fPOI.Keys.Where(char.IsLower);

        public Map(string map, Point2D door = null)
        {
            fWalkableTiles = ToTiles(map)
                .Where(t => t.Char is not WALL and not DOOR)
                .Append((Position: door ?? FindDoor(map), Char: DOOR))
                .ToDictionary(x => x.Position, x => x.Char);
            fPOI = fWalkableTiles.Where(kvp => kvp.Value != FLOOR).ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            foreach (var k in fPOI.Keys.ToList())
                if (k != DOOR && !TryGetPath(DOOR, k, out _))
                    fPOI.Remove(k);
        }

        private static IEnumerable<(Point2D Position, char Char)> ToTiles(string map)
            => map.Lines().SelectMany((line, y) => line.Select((c, x) => (new Point2D(x, y), c)));

        private static Point2D FindDoor(string map)
            => ToTiles(map).Where(x => x.Char == DOOR).Single().Position;

        public static Map[] Split(string map)
        {
            var doorPos = FindDoor(map);
            var lines = map.Lines().ToArray();
            lines[doorPos.Y - 1] = lines[doorPos.Y - 1].Remove((int)(doorPos.X - 1), 3).Insert((int)(doorPos.X - 1), "@#@");
            lines[doorPos.Y + 0] = lines[doorPos.Y + 0].Remove((int)(doorPos.X - 1), 3).Insert((int)(doorPos.X - 1), "###");
            lines[doorPos.Y + 1] = lines[doorPos.Y + 1].Remove((int)(doorPos.X - 1), 3).Insert((int)(doorPos.X - 1), "@#@");
            map = string.Join(Environment.NewLine, lines);

            var offsets = new Point2D[] { (-1, -1), (-1, +1), (+1, -1), (+1, +1) };
            var maps = offsets.Select(o => new Map(map, doorPos + o)).ToArray();
            return maps;
        }

        public (long Distance, string RequiredKeys) GetPath(char from, char to)
        {
            if (!TryGetPath(from, to, out var result))
                throw new InvalidOperationException("Invalid Waypoint");
            return result;
        }

        private bool TryGetPath(char from, char to, out (long Distance, string RequiredKeys) result)
        {
            var key = (from, to);
            if (!fPaths.TryGetValue(key, out var r))
                fPaths[key] = r = CalcPath(from, to);
            result = r ?? (0, string.Empty);
            return r.HasValue;
        }

        public (long Distance, string RequiredKeys)? CalcPath(char from, char to)
        {
            var open = new Queue<(Point2D, long, string)>();
            var visited = new HashSet<Point2D>();

            open.Enqueue((fPOI[from], 0, string.Empty));

            while (open.TryDequeue(out var current))
            {
                var (pos, dist, keys) = current;
                if (!visited.Add(pos))
                    continue;
                dist++;

                foreach (var n in pos.Neighbours())
                {
                    if (!fWalkableTiles.TryGetValue(n, out char nchar))
                        continue;
                    if (nchar == to)
                        return (dist, keys);

                    var nkeys = keys;

                    if (char.IsUpper(nchar))
                        nkeys = AppendKey(nkeys, char.ToLowerInvariant(nchar));

                    open.Enqueue((n, dist, nkeys));
                }
            }
            return null;
        }
    }

    private static string AppendKey(string keys, char key)
    {
        for (int i = 0; i < keys.Length; i++)
            if (keys[i] > key)
                return keys.Insert(i, key.ToString());
        return keys + key;
    }

    private static bool HasAllKeys(string requiredKeys, string collectedKeys)
    {
        if (requiredKeys.Length > collectedKeys.Length)
            return false;
        int c = 0;
        foreach (var req in requiredKeys)
        {
            while (collectedKeys[c] < req)
            {
                if (++c >= collectedKeys.Length)
                    return false;
            }
            if (collectedKeys[c] != req)
                return false;
        }
        return true;
    }

    private static long ShortestPath(string map, bool splitMap)
    {
        var maps = splitMap ? Map.Split(map) : new[] { new Map(map) };

        var dependencies = maps.SelectMany(map => map.Keys.Select(k => (Key: k, Required: map.GetPath(DOOR, k).RequiredKeys))).ToDictionary(k => k.Key, k => k.Required);
        var mapsByKey = maps.SelectMany((map, i) => map.Keys.Select(k => (k, i))).ToDictionary(x => x.k, x => x.i);

        var visited = new HashSet<(string Locations, string Keys)>();
        var open = new MinHeap<(string Locations, string Keys, long Distance)>(
            ComparerBuilder<(string Locations, string Keys, long Distance)>.CompareBy(x => x.Distance)
        );
        open.Push((new string(DOOR, maps.Length), string.Empty, 0));

        while (open.TryPop(out var currentItem))
        {
            var (location, collectedKeys, distance) = currentItem;
            if (collectedKeys.Length == dependencies.Count)
                return distance;
            if (!visited.Add((location, collectedKeys)))
                continue;

            var candidates = dependencies.Where(kvp => !collectedKeys.Contains(kvp.Key) && HasAllKeys(kvp.Value, collectedKeys)).Select(kvp => kvp.Key);
            foreach (var candidate in candidates)
            {
                var mapId = mapsByKey[candidate];
                var newLocations = new StringBuilder(location.Length);
                for (int i = 0; i < location.Length; i++)
                    newLocations.Append(i == mapId ? candidate : location[i]);

                open.Push((newLocations.ToString(), AppendKey(collectedKeys, candidate), distance + maps[mapId].GetPath(location[mapId], candidate).Distance));
            }
        }

        return -1;
    }

    protected override long? Part1()
    {
        Assert(ShortestPath(Sample("01"), false), 8);
        Assert(ShortestPath(Sample("02"), false), 86);
        Assert(ShortestPath(Sample("03"), false), 132);
        Assert(ShortestPath(Sample("04"), false), 136);
        Assert(ShortestPath(Sample("05"), false), 81);
        return ShortestPath(Input, false);
    }

    protected override long? Part2()
    {
        Assert(ShortestPath(Sample("11"), true), 8);
        Assert(ShortestPath(Sample("12"), true), 24);
        Assert(ShortestPath(Sample("13"), true), 32);
        Assert(ShortestPath(Sample("14"), true), 72);
        return ShortestPath(Input, true);
    }
}
