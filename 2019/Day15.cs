﻿namespace AdventOfCode._2019;

using Point = Point2D<int>;

class Day15 : Solution
{
    enum SensorInput { Wall = 0, Moved = 1, OxygenSystem = 2 };
    enum Movement { North = 1, South = 2, West = 3, East = 4 };
    private static readonly Point[] MoveDirections = [Point.Origin, Point.Up, Point.Down, Point.Left, Point.Right];
    private static (IntCodeVM, SensorInput) StepVM(IntCodeVM vm, Movement move)
    {
        while (true)
        {
            long? output;
            (output, vm) = vm.Step(() => (long)move);
            if (output.HasValue)
                return (vm, (SensorInput)output);
        }
    }

    private IEnumerable<(SensorInput Type, ImmutableList<Movement> Inputs)> GetWalkableTiles(ImmutableList<Movement> initialMovements)
    {
        var visited = new HashSet<Point>();
        var queue = new Queue<(Point Location, ImmutableList<Movement> Inputs, IntCodeVM VM)>();

        var initialVM = initialMovements.Aggregate(new IntCodeVM(Input), (vm, m) =>
        {
            (vm, _) = StepVM(vm, m);
            return vm;
        });

        queue.Enqueue((Point.Origin, ImmutableList<Movement>.Empty, initialVM));
        visited.Add(Point.Origin);

        while (queue.TryDequeue(out var state))
        {
            foreach (var dir in Enum.GetValues<Movement>())
            {
                var newLocation = state.Location + MoveDirections[(int)dir];
                if (!visited.Add(newLocation))
                    continue;

                var inputs = state.Inputs.Add(dir);
                var (vm, sensor) = StepVM(state.VM, dir);
                if (sensor == SensorInput.Wall)
                    continue;

                yield return (sensor, inputs);
                queue.Enqueue((newLocation, inputs, vm));
            }
        }
    }

    private ImmutableList<Movement> GetCommandsForOxygenTank()
        => GetWalkableTiles([]).First(n => n.Type == SensorInput.OxygenSystem).Inputs;

    protected override long? Part1()
        => GetCommandsForOxygenTank().Count;

    protected override long? Part2()
        => GetWalkableTiles(GetCommandsForOxygenTank()).Max(i => i.Inputs.Count);
}
