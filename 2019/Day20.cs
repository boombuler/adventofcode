namespace AdventOfCode._2019;

using System.Collections.Frozen;

class Day20 : Solution
{
    record Portal(string Name, Point2D OtherPortal, int Direction);

    private static (FrozenDictionary<Point2D, Portal> Portals, FrozenSet<Point2D> WalkableTiles) ParseMap(string map)
    {
        var mapLines = map.Lines().ToArray();
        char GetCh(Point2D pt) => mapLines[(int)pt.Y][(int)pt.X];
        var width = mapLines.Max(l => l.Length) - 2;
        var height = mapLines.Length - 2;
        var walkable = new HashSet<Point2D>();
        var innerPortals = new Dictionary<string, Point2D>();
        var outerPortals = new Dictionary<string, Point2D>();
        var portPos = new Dictionary<Point2D, (string Name, Dictionary<string, Point2D> Other)>();

        foreach (var pos in Point2D.Range((1, 1), (width, height)))
        {
            var ch = GetCh(pos);
            switch (ch)
            {
                case ' ':
                case '#':
                    continue;
                case '.':
                    walkable.Add(pos); break;
                default:
                    var waypoint = pos.Neighbours().FirstOrDefault(p => GetCh(p) == '.');
                    if (waypoint == null)
                        continue;

                    var otherLetterPos = pos - (waypoint - pos);
                    var otherLetter = GetCh(otherLetterPos);
                    var name = pos.CompareTo(otherLetterPos) < 0 ? new string(new char[] { ch, otherLetter }) : new string(new char[] { otherLetter, ch });
                    var (targetRing, otherRing) = (pos.X <= 1 || pos.Y <= 1 || pos.X >= width || pos.Y >= height) ? (outerPortals, innerPortals) : (innerPortals, outerPortals);
                    targetRing[name] = waypoint;
                    portPos[waypoint] = (name, otherRing);
                    break;
            }
        }

        return (portPos.ToFrozenDictionary(
                kvp => kvp.Key,
                kvp => new Portal(kvp.Value.Name, kvp.Value.Other.GetValueOrDefault(kvp.Value.Name), kvp.Value.Other == innerPortals ? -1 : 1)
            ), walkable.ToFrozenSet());
    }

    private static int ShortestPath(string mapStr, bool recursive)
    {
        var (portals, walkable) = ParseMap(mapStr);
        var open = new Queue<(Point3D, int Distance)>();
        var closed = new HashSet<Point3D>();
        var start = portals.First(p => p.Value.Name == "AA").Key.WithZ(0);
        var offset = recursive ? 1 : 0;

        open.Enqueue((start, 0));
        while (open.TryDequeue(out var current))
        {
            var (node, dist) = current;

            if (node.Z < 0 || !closed.Add(node))
                continue;

            var (pos, level) = node;

            if (portals.TryGetValue(pos, out var portal))
            {
                switch (portal.Name)
                {
                    case "AA": break;
                    case "ZZ":
                        if (level == 0)
                            return dist;
                        break;
                    default:
                        open.Enqueue((portal.OtherPortal.WithZ(level + (portal.Direction * offset)), dist + 1));
                        break;
                }
            }

            foreach (var n in pos.Neighbours().Where(walkable.Contains))
                open.Enqueue((n.WithZ(level), dist + 1));
        }
        return -1;
    }

    protected override long? Part1()
    {
        Assert(ShortestPath(Sample("1"), false), 23);
        Assert(ShortestPath(Sample("2"), false), 58);
        return ShortestPath(Input, false);
    }

    protected override long? Part2()
    {
        Assert(ShortestPath(Sample("1"), true), 26);
        Assert(ShortestPath(Sample("3"), true), 396);
        return ShortestPath(Input, true);
    }
}
