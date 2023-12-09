namespace AdventOfCode._2022;

class Day22 : Solution
{
    private static readonly Point2D[] Directions = [Point2D.Right, Point2D.Down, Point2D.Left, Point2D.Up];
    delegate (int Direction, Point2D Position) GetPositionDelegate(int direction, Point2D Pt);
    class CubeFace(Point2D mapIndex)
    {
        public (CubeFace Face, int Direction)[] Sides { get; } = new (CubeFace Face, int Rotate)[Directions.Length];
        public Point2D MapIndex { get; } = mapIndex;
    }

    private static long GetFinalPositionOnMap(string input)
        => GetFinalPosition(input, map =>
        {
            var columns = map.GroupBy(p => p.X).ToDictionary(g => g.Key, g => g.MinMax(p => p.Y));
            var rows = map.GroupBy(p => p.Y).ToDictionary(g => g.Key, g => g.MinMax(p => p.X));
            var coerceBounds = new Func<Point2D, Point2D>[]
            {
                n => n.X > rows[n.Y].max ? (rows[n.Y].min, n.Y) : n,
                n => n.Y > columns[n.X].max ? (n.X, columns[n.X].min) : n,
                n => n.X < rows[n.Y].min ? (rows[n.Y].max, n.Y) : n,
                n => n.Y < columns[n.X].min ? (n.X, columns[n.X].max) : n,
            };
            return (dir, pos) => (dir, coerceBounds[dir](pos + Directions[dir]));
        });

    private static Point2D GetFaceIndex(Point2D pt, int cubeSize) => pt / cubeSize;

    private static Dictionary<Point2D, CubeFace> FoldCube(IEnumerable<Point2D> points, int cubeSize)
    {
        var faces = points.Select(p => GetFaceIndex(p, cubeSize)).Distinct().ToDictionary(f => f, f => new CubeFace(f));
        foreach (var (s, face) in faces)
        {
            for (int d = 0; d < Directions.Length; d++)
            {
                if (faces.TryGetValue(s + Directions[d], out var other))
                    face.Sides[d] = (other, d);
            }
        }

        bool doneFolding;
        do
        {
            doneFolding = true;
            foreach (var (_, open) in faces)
            {
                for (int dirToNeighbour = 0; dirToNeighbour < Directions.Length; dirToNeighbour++)
                {
                    var dirOpen = (dirToNeighbour + 1) % Directions.Length;
                    var (neighbour, dir) = open.Sides[dirToNeighbour];

                    if (neighbour == null || open.Sides[dirOpen].Face != null)
                        continue;

                    var (side, newDir) = neighbour.Sides[(dir + 1) % Directions.Length];
                    if (side != null)
                        open.Sides[dirOpen] = (side, (newDir + Directions.Length - 1) % Directions.Length);
                    else
                        doneFolding = false;
                }
            }
        } while (!doneFolding);
        return faces;
    }

    private static long GetFinalPositionOnCube(string input, int cubeSize)
        => GetFinalPosition(input, map =>
        {
            var validPoints = map.ToHashSet();
            var cube = FoldCube(validPoints, cubeSize);
            
            return (dir, pos) =>
            {
                var newPos = pos + Directions[dir];
                var oldFaceIndex = GetFaceIndex(pos, cubeSize);
                if (validPoints.Contains(newPos) && GetFaceIndex(newPos, cubeSize) == oldFaceIndex)
                    return (dir, newPos); // Still on same cube face...

                var (newFace, newDir) = cube[oldFaceIndex].Sides[dir];

                var offset = dir switch
                {
                    0 /* R */ => pos.Y % cubeSize,
                    1 /* D */ => cubeSize - 1 - (pos.X % cubeSize),
                    2 /* L */ => cubeSize - 1 - (pos.Y % cubeSize),
                    _ /* U */ => pos.X % cubeSize
                };

                return (newDir, (newFace.MapIndex * cubeSize) + (newDir switch
                {
                    0 /* R */ => new Point2D(0, offset),
                    1 /* D */ => (cubeSize-1-offset , 0),
                    2 /* L */ => (cubeSize-1, cubeSize-1-offset),
                    _ /* U */ => (offset, cubeSize - 1)
                }));
            };
        });

    private static long GetFinalPosition(string input, Func<IEnumerable<Point2D>, GetPositionDelegate> buildGetPosFunc)
    {
        var (mapInput, (directionInput, _)) = input.Split("\n\n");
        var map = mapInput.Cells(c => c == '#', c => c != ' ');

        var position = new Point2D(map.Keys.Where(p => p.Y == 0).Min(p => p.X), 0);
        var dir = 0;
        
        var walls = map.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToHashSet();
        var getNextPos = buildGetPosFunc(map.Keys);

        using var sr = new StringReader(directionInput);

        while (true)
        {
            while (sr.TryReadWhile(char.IsDigit, out string sCount) && int.TryParse(sCount, out int count))
            {
                for (int i = 0; i < count; i++)
                {
                    var (nextDir, nextPt) = getNextPos(dir, position);
                    if (walls.Contains(nextPt))
                        break;
                    (dir, position) = (nextDir, nextPt);
                }
            }

            if (sr.TryRead(out char d))
                dir = (dir + (d == 'R' ? 1 : -1) + Directions.Length) % Directions.Length;
            else
                return (position.Y + 1) * 1000 + (position.X + 1) * 4 + dir;
        }
    }

    protected override long? Part1()
    {
        Assert(GetFinalPositionOnMap(Sample()), 6032);
        return GetFinalPositionOnMap(Input);
    }

    protected override long? Part2()
    {
        Assert(GetFinalPositionOnCube(Sample(), 4), 5031);
        return GetFinalPositionOnCube(Input, 50);
    }
}
