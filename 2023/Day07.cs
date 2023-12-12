namespace AdventOfCode._2023;

using static Parser;

class Day07 : Solution
{
    private const int HandSize = 5;
    private static int Ranking(int[] hand, bool withJoker)
    {
        if (withJoker && hand.Any(n => n > 0))
            hand = hand.Where(n => n > 0).ToArray();
        var grps = hand.GroupBy(i => i).Select(Enumerable.Count).ToArray();
        return (grps.Max() + (HandSize - hand.Length)) * HandSize
            + (HandSize - grps.Length);
    }

    private static long RateHands(string input, string cardOrder, bool withJoker)
        => input.Lines().Select((
            from hand in AnyChar(cardOrder).Select(cardOrder.IndexOf).Take(HandSize)
            from bid in Long.Token()
            select new {
                Bid = bid,
                Ranking = Ranking(hand, withJoker),
                Value = hand.Aggregate((a, b) => (a * cardOrder.Length) + b) 
            }).MustParse
        ).OrderBy(n => n.Ranking).ThenBy(n => n.Value)
        .Select((n, i) => (i + 1) * n.Bid).Sum();

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
