namespace AdventOfCode._2018;

class Day09 : Solution
{
    record Args(long PlayerCount, long MaxMarble);
    private static readonly Func<string, Args?> ParseArgs = new Regex(@"(?<PlayerCount>\d+) players; last marble is worth (?<MaxMarble>\d+) points").ToFactory<Args>();

    private static long GetWinningScore(string args, long factor = 1)
    {
        var a = ParseArgs(args) ?? throw new InvalidInputException();
        var circle = new LinkedList<long>([0]);
        var current = circle.First!;
        long[] scores = new long[a.PlayerCount];
        long max = a.MaxMarble * factor;
        for (long i = 1; i <= max; i++)
        {
            if (i % 23 == 0)
            {
                for (int seek = 0; seek < 6; seek++)
                    current = current.Previous ?? circle.Last!;
                scores[(i - 1) % a.PlayerCount] += i + (current.Previous ?? circle.Last!).Value;
                circle.Remove(current.Previous ?? circle.Last!);
            }
            else
            {
                current = current.Next ?? circle.First!;
                current = circle.AddAfter(current, i);
            }
        }

        return scores.Max();
    }

    protected override long? Part1()
    {
        Assert(GetWinningScore("9 players; last marble is worth 25 points"), 32);
        Assert(GetWinningScore("10 players; last marble is worth 1618 points"), 8317);
        Assert(GetWinningScore("13 players; last marble is worth 7999 points"), 146373);
        Assert(GetWinningScore("17 players; last marble is worth 1104 points"), 2764);
        Assert(GetWinningScore("21 players; last marble is worth 6111 points"), 54718);
        Assert(GetWinningScore("30 players; last marble is worth 5807 points"), 37305);
        return GetWinningScore(Input);
    }

    protected override long? Part2() => GetWinningScore(Input, 100);
}
