using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AdventOfCode._2018
{
    class Day13 : Solution<Point2D>
    {
        enum Direction { Up = 0, Right = 1, Down = 2, Left = 3, MASK = Left, Invalid = -1 }
        private static Point2D[] DirectionOffsets = new Point2D[]
        {
            (0, -1), (0, 1), (-1, 0), (1, 0)
        };
        enum TurnDirection { Left = -1, Straight = 0, Right = 1 }

        record Cart(Point2D Position, Direction Direction, TurnDirection NextTurnDirection);

        delegate Cart Rail(Cart cart);

        private static Cart MoveStraight(Cart cart)
            => cart with { Position = cart.Position + DirectionOffsets[(int)cart.Direction] };
        private static Cart CurveSlash(Cart cart)
            => MoveStraight(cart with { Direction = (Direction)((int)cart.Direction ^ 1) });
        private static Cart CurveBackslash(Cart cart)
            => MoveStraight(cart with { Direction = (Direction)((int)cart.Direction ^ 3) });
        private static Cart Intersection(Cart cart)
            => MoveStraight(cart with
                {
                    Direction = (Direction)(((int)cart.Direction + (int)cart.NextTurnDirection) & (int)Direction.MASK),
                    NextTurnDirection = cart.NextTurnDirection switch
                    {
                        TurnDirection.Left => TurnDirection.Straight,
                        TurnDirection.Straight => TurnDirection.Right,
                        TurnDirection.Right => TurnDirection.Left,
                        _ => throw new InvalidOperationException()
                    }
                });

        private IEnumerable<(IEnumerable<Cart> RemainingCarts, IEnumerable<Point2D> CrashLocations)> Simulate(string map)
        {
            var (rails, carts) = ReadMap(map);

            while (carts.Count > 1)
            {
                var toMove = new Queue<Cart>(carts.OrderBy(c => c.Position.Y).ThenBy(c => c.Position.X));
                carts = ImmutableList<Cart>.Empty;
                var crashes = ImmutableList<Point2D>.Empty;
                while (toMove.TryDequeue(out var cart))
                {
                    var newCart = rails[cart.Position](cart);
                    if (toMove.Union(carts).Select(c => c.Position).Contains(newCart.Position))
                    {
                        toMove = new Queue<Cart>(toMove.Where(m => !Equals(m.Position, newCart.Position)));
                        carts = carts.RemoveAll(c => Equals(c.Position, newCart.Position));
                        crashes = crashes.Add(newCart.Position);
                    }
                    else
                        carts = carts.Add(newCart);
                }
                if (!crashes.IsEmpty)
                    yield return (carts, crashes);
            }
        }

        private static (Dictionary<Point2D, Rail> Rails, ImmutableList<Cart> Carts) ReadMap(string map)
        {
            var positions = map.Lines().SelectMany((l, y) => l
                .Select((c, x) => new { Char = c, Position = new Point2D(x, y) })
                .Where(c => !char.IsWhiteSpace(c.Char))
            );
            var cartDirections = new[] { '^', '>', 'v', '<' };
            return (
                Rails: positions.ToDictionary(p => p.Position, p => p.Char switch
                {
                    '+' => (Rail)Intersection,
                    '\\' => CurveBackslash,
                    '/' => CurveSlash,
                    _ => MoveStraight
                }),
                Carts: positions
                    .Select(p => new Cart(p.Position, (Direction)Array.IndexOf(cartDirections, p.Char), TurnDirection.Left))
                    .Where(c => c.Direction != Direction.Invalid).ToImmutableList()
            );
        }

        private Point2D CrashLocation(string map)
            => Simulate(map).SelectMany(s => s.CrashLocations).First();

        private Point2D LastCarLocation(string map)
            => Simulate(map).Last().RemainingCarts.Single().Position;

        protected override Point2D Part1()
        {
            Assert(CrashLocation(Sample(nameof(Part1))), new Point2D(7, 3));
            return CrashLocation(Input);
        }

        protected override Point2D Part2()
        {
            Assert(LastCarLocation(Sample(nameof(Part2))), new Point2D(6,4));
            return LastCarLocation(Input);
        }
    }
}
