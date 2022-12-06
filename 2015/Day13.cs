namespace AdventOfCode._2015;

class Day13 : Solution
{
    private static readonly Regex ParseRule = new(@"(?<a>\w+) would (?<gol>gain|lose) (?<amount>\d+) happiness units by sitting next to (?<b>\w+)\.");
    private static Dictionary<(string, string), long> ParseRules(string rulesText)
    {
        var rules = new Dictionary<(string, string), long>();
        foreach (var line in rulesText.Lines())
        {
            var m = ParseRule.Match(line);
            if (!m.Success)
                throw new Exception();

            long amount = long.Parse(m.Groups["amount"].Value);
            if (m.Groups["gol"].Value == "lose")
                amount = -amount;

            var a = m.Groups["a"].Value;
            var b = m.Groups["b"].Value;

            rules[(a, b)] = rules.GetValueOrDefault((a, b)) + amount;
            rules[(b, a)] = rules.GetValueOrDefault((b, a)) + amount;
        }
        return rules;
    }

    private static long CheckBestSeatOrder(string inputFile, params string[] AdditionalPersons)
    {
        var rules = ParseRules(inputFile);
        var persons = AdditionalPersons.Union(rules.Keys.SelectMany(k => new string[] { k.Item1, k.Item2 })).ToArray();

        int max = persons.Length - 1;
        long maxHappyness = 0;

        void permutateSeatings(int index, long happyness)
        {
            if (index == max)
            {
                happyness += rules.GetValueOrDefault((persons[0], persons[index])) +
                             rules.GetValueOrDefault((persons[index - 1], persons[index]));
                if (happyness > maxHappyness)
                    maxHappyness = happyness;
            }
            else
            {
                for (int i = index; i <= max; i++)
                {
                    (persons[index], persons[i]) = (persons[i], persons[index]);
                    var diff = index == 0 ? 0 : rules.GetValueOrDefault((persons[index - 1], persons[index]));
                    permutateSeatings(index + 1, happyness + diff);
                    (persons[index], persons[i]) = (persons[i], persons[index]);
                }
            }
        }
        permutateSeatings(0, 0);
        return maxHappyness;
    }

    protected override long? Part1()
    {
        Assert(CheckBestSeatOrder(Sample()), 330);
        return CheckBestSeatOrder(Input);
    }

    protected override long? Part2() => CheckBestSeatOrder(Input, "Me");
}
