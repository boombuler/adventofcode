namespace AdventOfCode._2024;

using Point = Point2D<int>;

class Day21 : Solution
{
    class KeyPad
    {
        private readonly Point fHole;
        private readonly FrozenDictionary<char, Point> fKeyMap;
        private static readonly (char Char, Point Direction)[] Directions = "<v^>" // Direction Keys ordered by manhatten distance from the A key on the direction keypad.
            .Select(c => (c, Point.DirectionFromArrow(c))).ToArray();

        public KeyPad(FrozenDictionary<char, Point> keyMap)
        {
            fKeyMap = keyMap;
            fHole = fKeyMap['A'] with { X = 0 };
        }

        public string MovesFromString(string input) => string.Concat(input.Prepend('A').Pairwise(Transition));

        public string Transition(char from, char to)
        {
            var sb = new StringBuilder();
            var target = fKeyMap[to];
            var curPos = fKeyMap[from];
            var delta = target - curPos;
            var d = 0;

            while (delta != (0, 0))
            {
                var (dirChar, dir) = Directions[(d++ % Directions.Length)];
                var amount = dir.X == 0 ? delta.Y / dir.Y : delta.X / dir.X;
                if (amount <= 0)
                    continue;
                var dest = curPos + dir * amount;
                if (dest == fHole)
                    continue;
                curPos = dest;
                delta -= dir * amount;
                sb.Append(new string(dirChar, amount));
            }
            sb.Append('A');
            return sb.ToString();
        }
    }

    private static readonly KeyPad NumPad = new KeyPad(new Dictionary<char, Point>()
    {
        ['7'] = (0, 0), ['8'] = (1, 0), ['9'] = (2, 0),
        ['4'] = (0, 1), ['5'] = (1, 1), ['6'] = (2, 1),
        ['1'] = (0, 2), ['2'] = (1, 2), ['3'] = (2, 2),
                        ['0'] = (1, 3), ['A'] = (2, 3),
    }.ToFrozenDictionary());

    private static readonly KeyPad DirPad = new KeyPad(new Dictionary<char, Point>()
    {
                        ['^'] = (1, 0), ['A'] = (2, 0),
        ['<'] = (0, 1), ['v'] = (1, 1), ['>'] = (2, 1),
    }.ToFrozenDictionary());

    private readonly Dictionary<(char lastPos, char newPos, int level), long> fDPadMoveCounts = [];

    private long CountDirPadMoves(char lastPos, char newPos, int level)
    {
        return fDPadMoveCounts.GetOrAdd((lastPos, newPos, level), () =>
        {
            var todo = DirPad.Transition(lastPos, newPos);
            if (level == 1)
                return todo.Length;

            return todo.Prepend('A').Pairwise((l, n) => CountDirPadMoves(l, n, level - 1)).Sum();
        });
    }

    private long Solve(string input, int robotDirPads) => input.Lines().Sum(line => long.Parse(line[0..^1]) *
        NumPad.MovesFromString(line).Prepend('A').Pairwise((l, n) => CountDirPadMoves(l, n, robotDirPads)).Sum()
    );

    protected override long? Part1()
    {
        Assert(Solve(Sample(), 2), 126384);
        return Solve(Input, 2);
    }

    protected override long? Part2() => Solve(Input, 25);
}
