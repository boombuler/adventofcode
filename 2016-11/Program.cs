using AdventHelper;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace _2016_11
{
    class Program : ProgramBase
    {
        static void Main(string[] args) => new Program().Run();

        class State
        {
            private readonly ImmutableList<string> Floors;
            private readonly int ElevatorPos;
            private readonly int LastElevatorPos;
            public int NumberOfMoves { get; }
            public bool IsValid => ValidFloor(Floors[ElevatorPos]) && ValidFloor(Floors[LastElevatorPos]);

            public State(ImmutableList<string> floors)
                : this(floors, 0, 0, 0)
            { 
            }
            private State(ImmutableList<string> floors, int moveCount, int elevatorPos, int lastElevatorPos)
            {
                Floors = floors;
                NumberOfMoves = moveCount;
                ElevatorPos = elevatorPos;
                LastElevatorPos = lastElevatorPos;
            }

            public bool Success()
            {
                if (ElevatorPos != Floors.Count - 1)
                    return false;
                for (int i = 0; i < (Floors.Count - 1); i++)
                    if (!string.IsNullOrEmpty(Floors[i]))
                        return false;
                return true;
            }

            private static bool ValidFloor(string s)
            {
                var generators = s.Where(char.IsUpper);

                if (generators.Any())
                {
                    var chips = s.Where(char.IsLower);
                    foreach (var chip in chips)
                    {
                        if (!generators.Contains(char.ToUpperInvariant(chip)))
                            return false;
                    }
                }
                return true;
            }

            

            private State Move(int dir, int i1, int? i2 = null)
            {
                var curFloor = Floors[ElevatorPos];
                var newCurFloor = new string(curFloor.Where((_, i) => i != i1 && i != i2).ToArray());
                var otherFloor = Floors[ElevatorPos + dir];
                var newOtherFloor = otherFloor.Append(curFloor[i1]);
                if (i2.HasValue)
                    newOtherFloor = newOtherFloor.Append(curFloor[i2.Value]);

                var newFloors = Floors
                    .SetItem(ElevatorPos, newCurFloor).SetItem(ElevatorPos + dir, new string(newOtherFloor.OrderBy(c => c).ToArray()));
                return new State(newFloors, NumberOfMoves + 1, ElevatorPos + dir, ElevatorPos);
            }

            private IEnumerable<int> PossibleElevatorDirections()
            {
                if (ElevatorPos < (Floors.Count - 1))
                    yield return 1;
                if (ElevatorPos > 0)
                    yield return -1;
            }

            public IEnumerable<State> NextMoves()
            {
                var curFloor = Floors[ElevatorPos];
                for (int i1 = 0; i1 < curFloor.Length; i1++)
                {
                    foreach(var dir in PossibleElevatorDirections())
                    {
                        yield return Move(dir, i1);
                        for (int i2 = i1 + 1; i2 < curFloor.Length; i2++)
                            yield return Move(dir, i1, i2);
                    }
                }
            }

            public override string ToString() => ElevatorPos.ToString() + string.Join("|", Floors);
        }


        private long Solve(string notes)
        {
            var floors = ReadLines(notes).Select(note =>
            {
                note = note.Substring(note.IndexOf("contains") + "contains".Length).Replace(" and ", ",");
                var itms = note.Split(',');
                return new string(itms
                    .Select(itm => itm.Trim(' ', '.'))
                    .Where(itm => itm.StartsWith("a ") || itm.StartsWith("an "))
                    .Select(itm => itm.Substring(2).Split(' ', '-'))
                    .Select(parts => new { Element = parts.First().First(), Generator = parts.Last() == "generator" })
                    .Select(itm => itm.Generator ? char.ToUpperInvariant(itm.Element) : char.ToLowerInvariant(itm.Element))
                    .ToArray()
                );
            }).ToImmutableList();


            var states = new Queue<State>();
            states.Enqueue(new State(floors));
            var checkedMoves = new HashSet<string>();
            while (states.TryDequeue(out var state))
            {
                if (state.Success())
                    return state.NumberOfMoves;
                else
                {
                    foreach(var move in state.NextMoves())
                    {
                        if (move.IsValid && checkedMoves.Add(move.ToString()))
                            states.Enqueue(move);
                    }
                }
            }

            return long.MaxValue;
        }


        protected override long? Part1()
        {
            Assert(Solve("Sample"), 11);
            return Solve("Input");
        }

        protected override long? Part2() => Solve("Input2");
    }
}
