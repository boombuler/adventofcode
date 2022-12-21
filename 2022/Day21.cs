namespace AdventOfCode._2022;

partial class Day21 : Solution
{
    const string ROOT = "root";
    const string HUMAN = "humn";

    interface IMonkey
    {
        string Name { get; }
        long CalcValue(IDictionary<string, IMonkey> monkeys);
        bool RequiresHumanInput(IDictionary<string, IMonkey> monkeys);
    }

    partial record ValueMonkey(string Name, long Value) : IMonkey
    {
        public bool RequiresHumanInput(IDictionary<string, IMonkey> monkeys) => Name == HUMAN;
        public long CalcValue(IDictionary<string, IMonkey> monkeys) => Value;
        
        [GeneratedRegex("(?<Name>\\w+): (?<Value>-?\\d+)")]
        public static partial Regex GetParser();
    }

    partial record ExpressionMonkey(string Name, string Left, string Right, char Operation) : IMonkey
    {
        [GeneratedRegex(@"(?<Name>\w+): (?<Left>\w+) (?<Operation>[+\-*/]) (?<Right>\w+)")]
        public static partial Regex GetParser();

        public long CalcValue(IDictionary<string, IMonkey> monkeys)
        {
            var l = monkeys[Left].CalcValue(monkeys);
            var r = monkeys[Right].CalcValue(monkeys);
            return Operation switch
            {
                '+' => l + r,
                '-' => l - r,
                '*' => l* r,
                '/' => l / r,
                _ => throw new NotImplementedException()
            };
        }
        public bool RequiresHumanInput(IDictionary<string, IMonkey> monkeys)
            => monkeys[Left].RequiresHumanInput(monkeys) || monkeys[Right].RequiresHumanInput(monkeys);
    }

    private IEnumerable<IMonkey> GetMonkeys(string input)
    {
        foreach (var line in input.Lines())
        {
            if (ValueMonkey.GetParser().TryMatch(line, out ValueMonkey vm))
                yield return vm;
            else if (ExpressionMonkey.GetParser().TryMatch(line, out ExpressionMonkey em))
                yield return em;
            else
                throw new InvalidOperationException("Unknown Monkey");
        }
    }

    private long GetRootValue(string input)
    {
        var monkeys = GetMonkeys(input).ToDictionary(m => m.Name);
        return monkeys[ROOT].CalcValue(monkeys);
    }

    private long GetHumanValue(string input)
    {
        var monkeys = GetMonkeys(input).ToDictionary(m => m.Name);
        var root = monkeys[ROOT] as ExpressionMonkey;
        var (hum, fixVal) = monkeys[root.Left].RequiresHumanInput(monkeys) ? (root.Left, root.Right) : (root.Right, root.Left);
        var targetValue = monkeys[fixVal].CalcValue(monkeys);
        var current = monkeys[hum];
        while (current is ExpressionMonkey em)
        {
            var (l, r) = (monkeys[em.Left], monkeys[em.Right]);
            var humanLeft = l.RequiresHumanInput(monkeys);
            switch (em.Operation)
            {
                case '+':
                    (l, r) = humanLeft ? (l, r) : (r, l);
                    targetValue -= r.CalcValue(monkeys);
                    current = l;
                    break;
                case '*':
                    (l, r) = humanLeft ? (l, r) : (r, l);
                    targetValue /= r.CalcValue(monkeys);
                    current = l;
                    break;
                case '/' when humanLeft:
                    targetValue *= r.CalcValue(monkeys);
                    current = l;
                    break;
                case '/':
                    targetValue = l.CalcValue(monkeys) / targetValue;
                    current = r;
                    break;
                case '-' when humanLeft:
                    targetValue += r.CalcValue(monkeys);
                    current = l;
                    break;
                case '-':
                    targetValue = l.CalcValue(monkeys) - targetValue;
                    current = r;
                    break;
            }
        }
        return targetValue;
    }

    protected override long? Part1()
    {
        Assert(GetRootValue(Sample()), 152);
        return GetRootValue(Input);
    }

    protected override long? Part2()
    {
        Assert(GetHumanValue(Sample()), 301);
        return GetHumanValue(Input);
    }
}
