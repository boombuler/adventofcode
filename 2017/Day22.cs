using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2017
{
    class Day22 : Solution
    {
        public enum State
        {
            Clean = 0, //  nodes become weakened.
            Weakened = 1, // nodes become infected.
            Infected = 2, //  nodes become flagged.
            Flagged = 3, //  nodes become clean
        }
        enum TurnDirection: int { Left = -1, None = 0, Right = 1, Reverse = 2 }

        private void HandleCarrier(string map, int burstCount, Func<State, (State, TurnDirection)> update)
        {
            var mapGrid = map.Lines().ToList();
            var yOffset = -(mapGrid.Count / 2);
            var nodeStates = new Dictionary<Point2D, State>();
            for (int y = 0; y < mapGrid.Count; y++)
            {
                var line = mapGrid[y];
                var xOffSet = -(line.Length / 2);
                foreach (var infection in line.Select((c, x) => (c, x)).Where(t => t.c == '#').Select(t => new Point2D(t.x + xOffSet, y + yOffset)))
                    nodeStates[infection] = State.Infected;
            }

            var pos = Point2D.Origin;
            int direction = 0;
            var offsets = new Point2D[]
            {
                (0, -1), // Up
                (1, 0), // Right
                (0, 1), // Down
                (-1, 0) // Left
            };

            for (int i = 0; i < burstCount; i++)
            {
                if (!nodeStates.TryGetValue(pos, out State s))
                    s = State.Clean;

                (var newState, var dir) = update(s);
                nodeStates[pos] = newState;
                direction = (direction + (int)dir) & 3;
                pos += offsets[direction];
            }
        }

        private long VirusVersion1(string map, int burstCount)
        {
            long count = 0;
            HandleCarrier(map, burstCount, s =>
            {
                if (s == State.Clean)
                {
                    count++;
                    return (State.Infected, TurnDirection.Left);
                }
                return (State.Clean, TurnDirection.Right);
            });
            return count;
        }

        private long VirusVersion2(string map, int burstCount)
        {
            long count = 0;
            HandleCarrier(map, burstCount, s =>
            {
                switch(s)
                {
                    case State.Clean: return (State.Weakened, TurnDirection.Left);
                    case State.Weakened:
                        count++;
                        return (State.Infected, TurnDirection.None);
                    case State.Infected: return (State.Flagged, TurnDirection.Right);
                    case State.Flagged: return (State.Clean, TurnDirection.Reverse);
                    default: throw new InvalidOperationException();
                }
            });
            return count;
        }


        protected override long? Part1()
        {
            Assert(VirusVersion1(Sample(), 70), 41);
            Assert(VirusVersion1(Sample(), 10000), 5_587);
            return VirusVersion1(Input, 10_000);
        }

        protected override long? Part2()
        {
            Assert(VirusVersion2(Sample(), 100), 26);
            Assert(VirusVersion2(Sample(), 10_000_000), 2_511_944);
            return VirusVersion2(Input, 10_000_000);
        }
    }
}
