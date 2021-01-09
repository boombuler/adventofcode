using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode._2016
{
    class Day11 : Solution
    {
        class State
        {
            private static IEnumerable<State> NONE = Enumerable.Empty<State>();
            private readonly ImmutableList<string> Floors;
            private readonly int ElevatorPos;
            public int NumberOfMoves { get; }
            public State(ImmutableList<string> floors)
                : this(floors, 0, 0)
            { 
            }
            private State(ImmutableList<string> floors, int moveCount, int elevatorPos)
            {
                Floors = floors;
                NumberOfMoves = moveCount;
                ElevatorPos = elevatorPos;
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
                if (s.Any(char.IsUpper))
                {
                    foreach (var chip in s.ToUpperInvariant())
                    {
                        if (!s.Contains(chip))
                            return false;
                    }
                }
                return true;
            }


            private State Move(int dir, int i1, int? i2 = null)
            {
                var curFloor = Floors[ElevatorPos];
                var newCurFloor = new string(curFloor.Where((_, i) => i != i1 && i != i2).ToArray());
                if (!ValidFloor(newCurFloor))
                    return null;
                
                var newOtherFloor = Floors[ElevatorPos + dir].Append(curFloor[i1]);
                if (i2.HasValue)
                    newOtherFloor = newOtherFloor.Append(curFloor[i2.Value]);

                var other = new string(newOtherFloor.OrderBy(c => c).ToArray());
                
                if (!ValidFloor(other))
                    return null;

                var newFloors = Floors.SetItem(ElevatorPos, newCurFloor).SetItem(ElevatorPos + dir, other);
                return new State(newFloors, NumberOfMoves + 1, ElevatorPos + dir);
            }

            private IEnumerable<State> Move1(int dir) 
                => Enumerable.Range(0, Floors[ElevatorPos].Length)
                    .Select(i => Move(dir, i))
                    .Where(m => m != null);

            private IEnumerable<State> Move2(int dir)
            {
                var len = Floors[ElevatorPos].Length;
                for (int i1 = 0; i1 < len; i1++)
                    for (int i2 = i1 + 1; i2 < len; i2++)
                    {
                        var m = Move(dir, i1, i2);
                        if (m != null)
                            yield return m;
                    }    
            }

            public IEnumerable<State> GetValidMoves()
            {
                var moves = NONE;

                void AppendGenerator(int dir, Func<int, IEnumerable<State>> genA, Func<int, IEnumerable<State>> genB)
                {
                    var items = genA(dir);
                    if (items.Any())
                        moves = moves.Union(items);
                    else
                        moves = moves.Union(genB(dir));
                }

                if (ElevatorPos < (Floors.Count - 1))
                    AppendGenerator(+1, Move2, Move1);
                if (ElevatorPos > 0)
                    AppendGenerator(-1, Move1, Move2);

                return moves;
            }

            public override int GetHashCode()
            {
                var result = 17;
                result = (result * 23) + ElevatorPos.GetHashCode();
                for (int i = 0; i < Floors.Count; i++)
                    result = (result * 23) + Floors[i].GetHashCode();
                return result;
            }

            public override bool Equals(object obj)
            {
                if (obj is State other)
                    return ElevatorPos == other.ElevatorPos && Floors.SequenceEqual(other.Floors, StringComparer.Ordinal);
                return false;
            }
        }


        private long Solve(string notes)
        {
            var floors = notes.Lines().Select(note =>
            {
                note = note.Substring(note.IndexOf("contains") + "contains".Length).Replace(" and ", ",");
                var itms = note.Split(',');
                return new string(itms
                    .Select(itm => itm.Trim(' ', '.'))
                    .Where(itm => itm.StartsWith("a ") || itm.StartsWith("an "))
                    .Select(itm => itm.Substring(2).Split(' ', '-'))
                    .Select(parts => new { Element = parts.First().First(), Generator = parts.Last() == "generator" })
                    .Select(itm => itm.Generator ? char.ToUpperInvariant(itm.Element) : char.ToLowerInvariant(itm.Element))
                    .OrderBy(c => c)
                    .ToArray()
                );
            }).ToImmutableList();


            var states = new Queue<State>();
            states.Enqueue(new State(floors));
            var checkedMoves = new HashSet<State>();
            while (states.TryDequeue(out var state))
            {
                if (state.Success())
                    return state.NumberOfMoves;
                else
                {
                    foreach(var move in state.GetValidMoves())
                    {
                        if (checkedMoves.Add(move))
                            states.Enqueue(move);
                    }
                }
            }

            return long.MaxValue;
        }


        protected override long? Part1()
        {
            Assert(Solve(Sample()), 11);
            return Solve(Input);
        }

        protected override long? Part2()
        {
            const string firstFloorPrefix = "The first floor contains";
            const string extraComponents = " a elerium generator, a elerium-compatible microchip, a dilithium generator, a dilithium-compatible microchip,";
            var input = Input.Replace(firstFloorPrefix, firstFloorPrefix + extraComponents);
            return Solve(input);
        }
    }
}
