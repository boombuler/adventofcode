namespace AdventOfCode._2015;

class Day09 : Solution
{
    record Instruction(string From, string To, long Dist);
    private static readonly Func<string, Instruction> ParseInstruction
        = new Regex(@"(?<From>\w+) to (?<To>\w+) = (?<Dist>\d+)", RegexOptions.Compiled).ToFactory<Instruction>();

    private static IEnumerable<Instruction> ReadInstructions(string instructions)
    {
        foreach (var line in instructions.Lines())
        {
            var itm = ParseInstruction(line);
            yield return itm;
            yield return itm with { To = itm.From, From = itm.To };
        }
    }

    private static IEnumerable<long> PermutateRoutes(string fileName)
    {
        var instructions = ReadInstructions(fileName).ToList();

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
                if (!valid)
                    break;
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
