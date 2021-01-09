using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode._2020
{
    class Day23 : Solution
    {
        private const int CUPS_TO_PICKUP = 3;

        private LinkedListNode<T> NextCup<T>(LinkedListNode<T> linkedListNode)
            => linkedListNode.Next ?? linkedListNode.List.First;

        private LinkedListNode<int> MoveCups(LinkedList<int> cups, int moves)
        {
            var lookup = new Dictionary<int, LinkedListNode<int>>();
            var currentCup = cups.First;
            do
            {
                lookup[currentCup.Value] = currentCup;
            } while ((currentCup = currentCup.Next) != null);
            currentCup = cups.First;


            var pickedUp = new int[CUPS_TO_PICKUP];
            int maxValue = cups.Max();

            for (int round = 0; round < moves; round++)
            {
                var remove = NextCup(currentCup);
                for (int i = 0; i < CUPS_TO_PICKUP; i++)
                {
                    pickedUp[i] = remove.Value;
                    var removeNext = NextCup(remove);
                    cups.Remove(remove);
                    remove = removeNext;
                }
                
                int destValue = (currentCup.Value - 1);
                while (destValue == 0 || pickedUp.Contains(destValue))
                    destValue = (destValue == 0) ? maxValue : destValue - 1;
            
                var destNode = lookup[destValue];
                for (int i = 0; i < CUPS_TO_PICKUP; i++)
                {
                    var ins = lookup[pickedUp[i]];
                    cups.AddAfter(destNode, ins);
                    destNode = ins;
                }
                currentCup = NextCup(currentCup);
            }
            return lookup[1];
        }

        private long PlayGame(string input, int moves)
        {
            var cups = new LinkedList<int>(input.Select(c => c - '0'));
            
            var cur = NextCup(MoveCups(cups, moves));
            long result = 0;
            for (int i = 1; i < cups.Count; i++)
            {
                result = result * 10 + cur.Value;
                cur = NextCup(cur);
            }
            return result;
        }

        private long PlayLongGame(string input)
        {
            var cups = new LinkedList<int>(
                input.Select(c => c - '0').Union(
                    Enumerable.Range(input.Length + 1, 1_000_000 - input.Length)
                )
            );

            var cup = NextCup(MoveCups(cups, 10_000_000));
            return (long)cup.Value * NextCup(cup).Value;
        }

        protected override long? Part1()
        {
            Assert(PlayGame("389125467", 10), 92658374);
            Assert(PlayGame("389125467", 100), 67384529);
            return PlayGame(Input, 100);
        }

        protected override long? Part2()
        {
            Assert(PlayLongGame("389125467"), 149245887792);
            return PlayLongGame(Input);
        }
    }
}
