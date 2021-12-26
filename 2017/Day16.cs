namespace AdventOfCode._2017;

using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utils;

class Day16 : Solution<string>
{
    private IEnumerable<Action<char[]>> ParseDanceMoves()
    {
        foreach (var move in Input.Lines().First().Split(','))
        {
            switch (move[0])
            {
                case 's':
                    int offset = int.Parse(move[1..]);
                    yield return letters => letters.RotateRight(offset);
                    break;
                case 'x':
                    var x = move[1..].Split('/').Select(int.Parse).ToArray();
                    (var a, var b) = (x[0], x[1]);
                    yield return l => (l[a], l[b]) = (l[b], l[a]);
                    break;
                case 'p':
                    (var c1, var c2) = (move[1], move[3]);
                    yield return l =>
                    {
                        (int i1, int i2) = (Array.IndexOf(l, c1), Array.IndexOf(l, c2));
                        (l[i1], l[i2]) = (l[i2], l[i1]);
                    };
                    break;
            }
        }
    }

    private string Dance(int count = 1)
    {
        var moves = ParseDanceMoves().ToList();
        var seen = new Dictionary<string, int>();

        var letters = Enumerable.Range(0, 16).Select(offset => (char)('a' + offset)).ToArray();
        seen.Add(new string(letters), 0);

        for (int i = 1; i <= count; i++)
        {
            moves.ForEach(actn => actn(letters));
            var state = new string(letters);
            if (!seen.TryAdd(state, i))
            {
                var loop = i - seen[state];
                int remaining = (count - (i - 1));
                i += (remaining / loop) * loop;
            }
        }
        return new string(letters);
    }

    protected override string Part1() => Dance();
    protected override string Part2() => Dance(1_000_000_000);
}
