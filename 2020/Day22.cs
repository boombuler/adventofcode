namespace AdventOfCode._2020;

class Day22 : Solution
{
    private static IEnumerable<int> GetWinnerDeck(string input, bool recursive)
    {
        (bool p1Win, IEnumerable<int> Deck) Play(LinkedList<int> p1, LinkedList<int> p2)
        {

            var prevStates = new List<(int[] seq1, int[] seq2)>();
            bool CheckEndlessLoop()
            {
                var (s1, s2) = (p1.ToArray(), p2.ToArray());

                if (prevStates.Any(s => s.seq1.SequenceEqual(s1) || s.seq2.SequenceEqual(s2)))
                    return true;

                prevStates.Add((s1, s2));
                return false;
            }

            int rounds = 0;
            while (p1.Count > 0 && p2.Count > 0)
            {
                rounds++;
                if (recursive && CheckEndlessLoop())
                    return (true, p1);

                (var v1, var v2) = (p1.First.Value, p2.First.Value);
                p1.RemoveFirst(); 
                p2.RemoveFirst();

                bool p1Won = v1 > v2;
                if (recursive)
                {
                    if ((v1 <= p1.Count) && (v2 <= p2.Count))
                    {
                        var sub1 = new LinkedList<int>(p1.Take(v1));
                        var sub2 = new LinkedList<int>(p2.Take(v2));
                        (p1Won, _) = Play(sub1, sub2);
                    }
                }

                if (p1Won)
                {
                    p1.AddLast(v1);
                    p1.AddLast(v2);
                }
                else
                {
                    p2.AddLast(v2);
                    p2.AddLast(v1);
                }
            }
            if (p1.First == null)
                return (false, p2);
            return (true, p1);
        }

        var p1 = new LinkedList<int>(input.Lines().Skip(1).TakeWhile(l => !string.IsNullOrEmpty(l)).Select(int.Parse));
        var p2 = new LinkedList<int>(input.Lines().SkipWhile(l => !string.IsNullOrEmpty(l)).Skip(2).Select(int.Parse).ToList());

        return Play(p1, p2).Deck;
    }

    private static long GetWinningScore(string inputFile, bool recursive)
        => GetWinnerDeck(inputFile, recursive).Reverse().Select((v, i) => v * (i + 1)).Sum();

    protected override long? Part1()
    {
        Assert(GetWinningScore(Sample(), false), 306);
        return GetWinningScore(Input, false);
    }

    protected override long? Part2()
    {
        Assert(GetWinningScore(Sample(), true), 291);
        return GetWinningScore(Input, true);
    }
}
