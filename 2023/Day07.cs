namespace AdventOfCode._2023;

using static Parser;

class Day07 : Solution
{
    private static int Ranking(int[] hand, bool withJoker)
    {
        int handSize = hand.Length;
        if (withJoker && hand.Any(n => n > 0))
            hand = hand.Where(n => n > 0).ToArray();
        var grps = hand.GroupBy(i => i).Select(g => g.Count()).OrderDescending().ToList();
        return (grps[0] + (handSize - hand.Length)) * handSize 
            + (handSize - grps.Count);
    }

    private static long RateHands(string input, string cardOrder, bool withJoker)
        => input.Lines().Select((
            from hand in AnyChar(cardOrder).Select(cardOrder.IndexOf).Many1() + " "
            from bid in Int
            select (hand, bid)).MustParse
        ).OrderBy(n => Ranking(n.hand, withJoker)).ThenBy(n => n.hand.Aggregate((a,b) => (a*cardOrder.Length)+b))
        .Select((n, i) => (i + 1) * n.bid).Sum();

    protected override long? Part1()
    {
        static long Solve(string input) => RateHands(input, "23456789TJQKA", false);

        Assert(Solve(Sample()), 6440);
        return Solve(Input);
    }


    protected override long? Part2()
    {
        static long Solve(string input) => RateHands(input, "J23456789TQKA", true);

        Assert(Solve(Sample()), 5905);
        return Solve(Input);
    }
}
