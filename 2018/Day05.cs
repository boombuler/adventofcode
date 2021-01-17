using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2018
{
    class Day05 : Solution
    {
        private int React(IEnumerable<char> mol)
        {
            var molecule = new LinkedList<char>(mol);
            
            var node = molecule.First;
            while (node.Next != null)
            {
                if (char.ToUpperInvariant(node.Value) == char.ToUpperInvariant(node.Next.Value) &&
                    char.IsUpper(node.Value) != char.IsUpper(node.Next.Value))
                {
                    var prev = node.Previous;
                    molecule.Remove(node.Next);
                    molecule.Remove(node);
                    node = prev ?? molecule.First;
                }
                else
                    node = node.Next;
            }
            return molecule.Count;
        }

        private int ReactWithoutProblematicType(string molecule)
            => molecule.ToUpperInvariant().Distinct()
                .Select(c => molecule.Where(other => char.ToUpperInvariant(other) != c))
                .Select(React)
                .Min();

        protected override long? Part1()
        {
            Assert(React("dabAcCaCBAcCcaDA"), 10);
            return React(Input);
        }

        protected override long? Part2()
        {
            Assert(ReactWithoutProblematicType("dabAcCaCBAcCcaDA"), 4);
            return ReactWithoutProblematicType(Input);
        }
    }
}
