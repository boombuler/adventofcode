namespace AdventOfCode._2022;

class Day05 : Solution<string>
{
    record Command(int Count, int From, int To)
    {
        public static Func<string, Command> Factory 
            = new Regex(@"move (?<Count>\d+) from (?<From>\d+) to (?<To>\d+)").ToFactory<Command>();
    }

    private string BuildStacks(string input, Func<IEnumerable<char>, IEnumerable<char>> stackBehavior)
    {
        var stackCount = (input.Lines().First().Length + 1) / 4;
        var stacks = Enumerable.Range(1, stackCount).Select(_ => new Stack<char>()).ToArray();

        input.Lines().TakeWhile(l => l.TrimStart().StartsWith('['))
            .SelectMany(l => l.Chunk(4).Select((c, i) => new {Char = c[1], Stack = stacks[i] }).Where(n => n.Char != ' '))
            .Reverse()
            .ForEach(n => n.Stack.Push(n.Char));

        foreach (var command in input.Lines().SkipWhile(l => !string.IsNullOrEmpty(l)).Skip(1).Select(Command.Factory))
        {
            stackBehavior(Enumerable.Range(0, command.Count).Select(_ => stacks[command.From - 1].Pop()))
                .ForEach(stacks[command.To - 1].Push);
        }

        return new string(stacks.Select(s => s.Pop()).ToArray());
    }

    protected override string Part1()
    {
        Assert("CMZ", BuildStacks(Sample(), Functional.Identity));
        return BuildStacks(Input, Functional.Identity);
    }

    protected override string Part2()
    {
        Assert("MCD", BuildStacks(Sample(), Enumerable.Reverse));
        return BuildStacks(Input, Enumerable.Reverse);
    }
}
