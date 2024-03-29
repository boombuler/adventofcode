﻿namespace AdventOfCode._2018;

using Point = Point2D<int>;

class Day15 : Solution
{
    static readonly Point[] ReadingOrder = [Point.Up, Point.Left, Point.Right, Point.Down];
    private const int DEFAULT_ATTACK = 3;

    class Map
    {
        private readonly Dictionary<Entity, Point> fByEntity = [];
        private readonly Dictionary<Point, Entity> fByPosition = [];

        public Map(string input, int ElfAttack)
        {
            var ents = input.Lines().SelectMany((l, y) => l.Select((c, x) => (c, new Point(x, y))));
            foreach (var (c, pos) in ents)
            {
                switch (c)
                {
                    case '#': Add(new Wall(), pos); break;
                    case 'G': Add(new Creature(c, DEFAULT_ATTACK), pos); break;
                    case 'E': Add(new Creature(c, ElfAttack), pos); break;
                }
            }
        }

        private void Add(Entity ent, Point pos)
        {
            if (fByEntity.ContainsKey(ent) || fByPosition.ContainsKey(pos))
                return;
            fByPosition[pos] = ent;
            fByEntity[ent] = pos;
        }

        public Point this[Entity ent]
        {
            get => fByEntity[ent];
            set
            {
                var oldPos = fByEntity[ent];
                fByPosition.Remove(oldPos);
                fByPosition[value] = ent;
                fByEntity[ent] = value;
            }
        }
        public Entity this[Point pos] => fByPosition.TryGetValue(pos, out var res) ? res : null;
        public IEnumerable<Entity> Entities => fByEntity.Keys;
        public IEnumerable<Point> BlockedPositions => fByPosition.Keys;
        public bool Occupied(Point pt) => fByPosition.ContainsKey(pt);
        public void Remove(Entity ent)
        {
            var pos = fByEntity[ent];
            fByPosition.Remove(pos);
            fByEntity.Remove(ent);
        }
    }

    enum Team { Elves, Goblins }
    abstract class Entity { }
    sealed class Wall : Entity { }

    class Creature(char c, int atk) : Entity
    {
        public int DPH { get; } = atk;
        public Team Team { get; } = c == 'E' ? Team.Elves : Team.Goblins;
        public int HealthPoints { get; private set; } = 200;

        public Point MoveToTarget(Map map, HashSet<Point> targetPositions)
        {
            var curPos = map[this];

            int DistanceToFight(Point pt)
            {
                var visited = map.BlockedPositions.ToHashSet(); // Mark all entites as visited
                var moves = new List<Point>() { pt };
                var nextMoves = new List<Point>();
                int steps = 0;
                while (moves.Count > 0)
                {
                    foreach (var s in moves)
                    {
                        if (targetPositions.Contains(s))
                            return steps;
                        foreach (var ns in ReadingOrder.Select(dir => dir + s))
                            if (visited.Add(ns))
                                nextMoves.Add(ns);
                    }

                    steps++;
                    (moves, nextMoves) = (nextMoves, moves);
                    nextMoves.Clear();
                }
                return int.MaxValue;
            }

            var minDist = int.MaxValue;
            var nextPos = curPos;
            foreach (var move in ReadingOrder)
            {
                var p = curPos + move;
                if (map.Occupied(p))
                    continue;
                var dist = DistanceToFight(p);
                if (dist < minDist)
                {
                    minDist = dist;
                    nextPos = p;
                }
            }
            return map[this] = nextPos;
        }

        internal bool Hit(Creature target, Map map)
        {
            target.HealthPoints -= DPH;
            if (target.HealthPoints <= 0)
            {
                map.Remove(target);
                return true;
            }
            return false;
        }
    }

    private static (long Score, bool DeadElves) SimulateFight(string input, int elfAttackPower = DEFAULT_ATTACK, bool StopOnDeadElf = false)
    {
        var map = new Map(input, elfAttackPower);

        long round = 0;
        bool deadElves = false;

        while (true)
        {
            var moveOrder = map.Entities.OfType<Creature>()
                .OrderBy(cr => map[cr])
                .ToList();

            foreach (var creature in moveOrder)
            {
                if (creature.HealthPoints <= 0)
                    continue;

                var curPos = map[creature];
                var enemies = map.Entities.OfType<Creature>().Where(c => c.Team != creature.Team);
                if (!enemies.Any())
                    return (round * map.Entities.OfType<Creature>().Sum(c => c.HealthPoints), deadElves);
                var fightingPositions = enemies
                    .SelectMany(e => map[e].Neighbours())
                    .Where(p => Equals(p, curPos) || !map.Occupied(p))
                    .ToHashSet();

                if (fightingPositions.Count == 0)
                    continue;
                if (!fightingPositions.Contains(curPos))
                    curPos = creature.MoveToTarget(map, fightingPositions);
                if (fightingPositions.Contains(curPos))
                {
                    var target = ReadingOrder.Select(dir => map[dir + curPos]).OfType<Creature>().Where(t => t.Team != creature.Team)
                        .OrderBy(c => c.HealthPoints).ThenBy(c => map[c]).First();
                    if (creature.Hit(target, map) && target.Team == Team.Elves)
                    {
                        deadElves = true;
                        if (StopOnDeadElf)
                            return (int.MaxValue, deadElves);
                    }
                }
            }

            round++;
        }
    }

    protected override long? Part1()
    {
        Assert(SimulateFight(Sample(nameof(Part1))).Score, 27730);
        return SimulateFight(Input).Score;
    }

    private static long WithBoostedElves(string input)
        => EnumerableHelper.Generate(DEFAULT_ATTACK + 1)
            .Select(attk => SimulateFight(input, attk, StopOnDeadElf: true))
            .Where(r => !r.DeadElves)
            .Select(r => r.Score)
            .First();

    protected override long? Part2()
    {
        Assert(WithBoostedElves(Sample(nameof(Part2))), 1140);
        return WithBoostedElves(Input);
    }
}
