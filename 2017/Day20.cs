namespace AdventOfCode._2017;

class Day20 : Solution
{
    private class Particle
    {
        public int ID { get; }
        public Point3D Position { get; private set; }
        public Point3D Velocity { get; private set; }
        public Point3D Acceleration { get; private set; }
        private static readonly Func<string, Point3D> ParsePosition = new Regex(@"p=<(?<x>-?\d+),\s?(?<y>-?\d+),\s?(?<z>-?\d+)>", RegexOptions.Compiled).ToFactory<Point3D>();
        private static readonly Func<string, Point3D> ParseVelocity = new Regex(@"v=<(?<x>-?\d+),\s?(?<y>-?\d+),\s?(?<z>-?\d+)>", RegexOptions.Compiled).ToFactory<Point3D>();
        private static readonly Func<string, Point3D> ParseAcceleration = new Regex(@"a=<(?<x>-?\d+),\s?(?<y>-?\d+),\s?(?<z>-?\d+)>", RegexOptions.Compiled).ToFactory<Point3D>();
        public Particle(int id, string line)
        {
            ID = id;
            Position = ParsePosition(line);
            Velocity = ParseVelocity(line);
            Acceleration = ParseAcceleration(line);
        }

        public void Tick()
        {
            Velocity += Acceleration;
            Position += Velocity;
        }
    }

    private List<Particle> GetParticles() => Input.Lines().Select((p, i) => new Particle(i, p)).ToList();

    protected override long? Part1()
    {
        var origin = new Point3D(0, 0, 0);
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
            particles = particles.GroupBy(p => p.Position).Where(grp => grp.Count() == 1).Select(g => g.First()).ToList();
        }
        return particles.Count;
    }

}
