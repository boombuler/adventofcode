namespace AdventOfCode._2017;

using Point = Point3D<int>;
class Day20 : Solution
{
    private class Particle(int id, string line)
    {
        public int ID { get; } = id;
        public Point Position { get; private set; } = ParsePosition(line) ?? throw new InvalidInputException();
        public Point Velocity { get; private set; } = ParseVelocity(line) ?? throw new InvalidInputException();
        public Point Acceleration { get; } = ParseAcceleration(line) ?? throw new InvalidInputException();
        private static readonly Func<string, Point?> ParsePosition = new Regex(@"p=<(?<x>-?\d+),\s?(?<y>-?\d+),\s?(?<z>-?\d+)>", RegexOptions.Compiled).ToFactory<Point>();
        private static readonly Func<string, Point?> ParseVelocity = new Regex(@"v=<(?<x>-?\d+),\s?(?<y>-?\d+),\s?(?<z>-?\d+)>", RegexOptions.Compiled).ToFactory<Point>();
        private static readonly Func<string, Point?> ParseAcceleration = new Regex(@"a=<(?<x>-?\d+),\s?(?<y>-?\d+),\s?(?<z>-?\d+)>", RegexOptions.Compiled).ToFactory<Point>();

        public void Tick()
        {
            Velocity += Acceleration;
            Position += Velocity;
        }
    }

    private List<Particle> GetParticles() => [.. Input.Lines().Select((p, i) => new Particle(i, p))];

    protected override long? Part1()
    {
        var origin = new Point(0, 0, 0);
        return GetParticles()
            .OrderBy(p => p.Acceleration.ManhattanDistance(origin))
            .ThenBy(p => p.Velocity.ManhattanDistance(origin))
            .ThenBy(p => p.Position.ManhattanDistance(origin))
            .First().ID;
    }

    protected override long? Part2()
    {
        var particles = GetParticles();

        for (int round = 0; round < 500; round++)
        {
            particles.AsParallel().ForAll(p => p.Tick());
            particles = [.. particles.GroupBy(p => p.Position).Where(grp => grp.Count() == 1).Select(g => g.First())];
        }
        return particles.Count;
    }

}
