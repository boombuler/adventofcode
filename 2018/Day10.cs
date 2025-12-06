namespace AdventOfCode._2018;

using Point = Point2D<int>;
class Day10 : Solution<string, int>
{
    record Star
    {
        public Point Position { get; init; }
        private Point Velocity { get; }
        public Star(int PosX, int PosY, int VelX, int VelY)
            => (Position, Velocity) = ((PosX, PosY), (VelX, VelY));
        public Star Move() => this with { Position = Position + Velocity };
    }
    private static readonly Func<string, Star?> ParseStar = new Regex(@"position=<\s*(?<PosX>-?\d+),\s*(?<PosY>-?\d+)> velocity=<\s*(?<VelX>-?\d+),\s*(?<VelY>-?\d+)>").ToFactory<Star>();

    private static (string Text, int Second) Align(string stars)
    {
        var curStars = stars.Lines().Select(ParseStar).NonNull().ToArray();
        var next = new Star[curStars.Length];
        int seconds = 1;
        while (true)
        {
            for (int i = 0; i < curStars.Length; i++)
                next[i] = curStars[i].Move();
            var (minY, maxY) = next.MinMax(s => s.Position.Y);
            if (maxY - minY == OCR.CharHeight - 1)
            {
                var (minX, maxX) = next.MinMax(s => s.Position.X);

                var charMap = new char[maxX + 1 - minX, maxY + 1 - minY];
                foreach (var s in next)
                    charMap[s.Position.X - minX, s.Position.Y - minY] = '#';

                return (OCR.Decode(charMap), seconds);
            }

            (curStars, next) = (next, curStars);
            seconds++;
        }
    }

    private static OCR OCR => new OCR10x6();

    protected override string Part1() => Align(Input).Text;

    protected override int Part2() => Align(Input).Second;
}
