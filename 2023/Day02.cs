namespace AdventOfCode._2023;

using P = Parser;
class Day02 : Solution
{
    record Move(int Amount, Color Color)
    {
        public bool IsIllegal => Amount > MaxCubes[Color];
    }

    enum Color { red, green, blue };
    record Game(int Id, Move[] Moves);

    private static readonly Func<string, Game> ParseGame = 
            ("Game " + P.Int + ":")
            .Then(P.Regex<Move>(@"(?<Amount>\d+) (?<Color>[a-z]+)").List(',', ';', ' '),
                (id, moves) => new Game(id, moves)).MustParse;

    private static readonly FrozenDictionary<Color, int> MaxCubes = new Dictionary<Color, int>()
    {
        [Color.red] = 12,
        [Color.green] = 13,
        [Color.blue] = 14,
    }.ToFrozenDictionary();

    protected override long? Part1()
    {
        static long SumValidIds(string input)
            => input.Lines().Select(ParseGame).Where(g => !g.Moves.Any(m => m.IsIllegal)).Sum(g => g.Id);

        Assert(SumValidIds(Sample()), 8);
        return SumValidIds(Input);
    }

    protected override long? Part2()
    {
        static long GetPower(string input)
            => input.Lines().Select(ParseGame)
            .Sum(game => game.Moves.GroupBy(m => m.Color).Select(g => g.Max(m => m.Amount)).Aggregate((a, b) => a * b));

        Assert(GetPower(Sample()), 2286);
        return GetPower(Input);
    }
}
