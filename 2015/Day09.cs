namespace AdventOfCode._2015;

using static Parser;

class Day09 : Solution
{
    record Instruction(string From, string To, int Dist);
    private static readonly Func<string, Instruction> ParseInstruction =
        from f in Word.Token() + "to"
        from t in Word.Token() + "="
        from d in Int.Token()
        select new Instruction(f, t, d);

    private static IEnumerable<long> PermutateRoutes(string input)
    {
        var instructions = input.Lines().Select(ParseInstruction)
            .SelectMany<Instruction, Instruction>(i => [i, new(i.To, i.From, i.Dist)]).ToList();

        var cities = instructions.Select(i => i.From).Distinct();
        var distances = instructions.ToDictionary(v => (v.From, v.To), v => v.Dist);

        foreach (var route in cities.Permuatate())
        {
            long val = 0;
            bool valid = true;
            for (int i = 1; i < route.Length; i++)
            {
                if (distances.TryGetValue((route[i - 1], route[i]), out var d))
                    val += d;
                else
                {
                    valid = false;
                    break;
                }
            }
            if (valid)
                yield return val;
        }
    }

    protected override long? Part1()
    {
        Assert(PermutateRoutes(Sample()).Min(), 605);
        return PermutateRoutes(Input).Min();
    }

    protected override long? Part2()
    {
        Assert(PermutateRoutes(Sample()).Max(), 982);
        return PermutateRoutes(Input).Max();
    }
}
