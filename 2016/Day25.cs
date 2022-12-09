namespace AdventOfCode._2016;

using System.Threading.Channels;

class Day25 : Solution
{
    class VM : AssembunnyVM
    {
        public VM(string code)
            : base(code)
        {
        }

        public IEnumerable<int> RunCode(int a)
        {
            Registers[0] = a;
            var prevStates = new HashSet<(long a, long b, long c, long d, int pc)>();

            while (!Finished)
            {
                if (!prevStates.Add((Registers[0], Registers[1], Registers[2], Registers[3], PC)))
                    yield break; // since this state was already reached, everything after this would repeat the pattern.

                Operation(this);
                if (Output.TryRead(out var value))
                    yield return (int)value;

                PC++;
            }
        }
    }

    protected override long? Part1()
    {
        for (int i = 0; i < int.MaxValue; i++)
        {
            int last = -1;
            bool ok = true;
            int first = -1;
            foreach (var cur in new VM(Input).RunCode(i))
            {
                if (first == -1)
                    first = cur;
                else if ((last == 0 && cur != 1) || (last == 1 && cur != 0))
                {
                    ok = false;
                    break;
                }

                last = cur;
            }
            if (ok && last != first)
                return i;
        }
        return null;
    }

}
