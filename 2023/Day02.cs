namespace AdventOfCode._2023;

class Day02 : Solution
{
    private static readonly Func<string, Move> MoveFactory = new Regex(@"(?<Amount>\d+) (?<Color>[a-z]+)").ToFactory<Move>();
    record Move(int Amount, Color Color)
    {
        public bool IsIllegal => Amount > MaxCubes[Color];
    }

    enum Color { red, green, blue };

    private static readonly FrozenDictionary<Color, int> MaxCubes = new Dictionary<Color, int>()
    {
        [Color.red] = 12,
        [Color.green] = 13,
        [Color.blue] = 14,
    }.ToFrozenDictionary();

    private static (int Id, Move[] Moves) ParseGame(string gameStr)
    {
        var (g, (roundsStr, _)) = gameStr.Split(':');
        var rounds = roundsStr.Split([';', ',']).Select(m => MoveFactory(m.Trim())).ToArray();
        return (int.Parse(g.Split(' ').Last()), rounds);
    }

    protected override long? Part1()
    {
        static int GetIdIfLegal(string game)
        {
            var (id, moves) = ParseGame(game);
            return moves.Any(m => m.IsIllegal) ? 0 : id;
        }

        Assert(Sample().Lines().Sum(GetIdIfLegal), 8);
        return Input.Lines().Sum(GetIdIfLegal);
    }

    protected override long? Part2()
    {
        static int GetPower(string game)
        {
            var (_, moves) = ParseGame(game);
            return moves.GroupBy(m => m.Color).Select(g => g.Max(m => m.Amount)).Aggregate((a, b) => a * b);
        }

        Assert(Sample().Lines().Sum(GetPower), 2286);
        return Input.Lines().Sum(GetPower);
    }
}
