namespace AdventOfCode._2025;

using static Parser;

class Day01 : Solution
{
    private static IEnumerable<(int Clicks, int Pos)> Dial(string input) => input.Lines()
        .Select<string, (int Sign, int Value)>(AnyChar("LR", [-1, +1]).Then(Int))
        .Scan((Clicks: 0, Pos: 50), (last, move) =>
        {
            int oldPos = last.Pos - ((move.Sign + 1) * 50);                  // If moving forward, subtract 100 from the current position to get the sign change later
            int newPos = oldPos + (move.Sign * move.Value % 100);            // calculate the new position relative to the old position
            int landedOnZero = (((newPos | -newPos) >> 31) + 1);             // new position is equal to 0 
            int signFlipped = (((oldPos >> 31) & 1) ^ ((newPos >> 31) & 1)); // the move has crossed the 0 line 
            int notStartedAtZero = (last.Pos | -last.Pos) >> 31;             // check if we started at the zero point.
            int completeRevolutions = move.Value / 100;                      // if we move more then 100 steps, add a click for each batch of 100.
            return ((notStartedAtZero & (signFlipped | landedOnZero)) + completeRevolutions, MathExt.Mod(newPos, 100));
        });

    protected override long? Part1()
    {
        static long Solve(string input) => Dial(input).Count(n => n.Pos == 0);
        
        Assert(Solve(Sample()), 3);
        return Solve(Input);
    }

    protected override long? Part2()
    {
        static long Solve(string input) => Dial(input).Sum(n => n.Clicks);

        Assert(Solve(Sample()), 6);
        return Solve(Input);
    }
}
