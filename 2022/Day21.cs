namespace AdventOfCode._2022;

partial class Day21 : Solution
{
    const string ROOT = "root";
    const string HUMAN = "humn";

    interface IMonkey
    {
        long CalcValue();
        bool RequiresHumanInput();
        long SolveForHuman(long result);
    }

    partial record ValueMonkey(string Name, long Value) : IMonkey
    {
        public bool RequiresHumanInput() => Name == HUMAN;
        public long CalcValue() => Value;
        public long SolveForHuman(long result) => result;

        [GeneratedRegex("(?<Name>\\w+): (?<Value>-?\\d+)")]
        public static partial Regex GetParser();
    }

    partial record MonkeyExpression(string Name, string Left, string Right, char Operation)
    {
        [GeneratedRegex(@"(?<Name>\w+): (?<Left>\w+) (?<Operation>[+\-*/]) (?<Right>\w+)")]
        public static partial Regex GetParser();
    }

    class ExpressionMonkey : IMonkey
    {
        public IMonkey Left { get; set; }
        public IMonkey Right { get; set; }
        public MonkeyExpression Expression { get; init; }
        
        public long CalcValue()
        {
            var l = Left.CalcValue();
            var r = Right.CalcValue();
            return Expression.Operation switch
            {
                '+' => l + r,
                '-' => l - r,
                '*' => l * r,
                '/' => l / r,
                _ => throw new NotImplementedException()
            };
        }
        
        public bool RequiresHumanInput()
            => Left.RequiresHumanInput() || Right.RequiresHumanInput();

        public long SolveForHuman(long result)
        {
            var humanLeft = Left.RequiresHumanInput();
            if (Expression.Name == ROOT)
                return humanLeft ? Left.SolveForHuman(Right.CalcValue()) : Right.SolveForHuman(Left.CalcValue());
            switch (Expression.Operation)
            {
                case '+':
                    {
                        var (hum, fix) = humanLeft ? (Left, Right) : (Right, Left);
                        return hum.SolveForHuman(result - fix.CalcValue());
                    }
                case '*':
                    {
                        var (hum, fix) = humanLeft ? (Left, Right) : (Right, Left);
                        return hum.SolveForHuman(result / fix.CalcValue());
                    }
                case '/' when humanLeft:
                    return Left.SolveForHuman(result * Right.CalcValue());
                case '/':
                    return Right.SolveForHuman(Left.CalcValue() / result);
                case '-' when humanLeft:
                    return Left.SolveForHuman(result + Right.CalcValue());
                case '-':
                    return Right.SolveForHuman(Left.CalcValue() - result);
            }
            throw new InvalidOperationException();
        }
    }

    private static Dictionary<string, IMonkey> GetMonkeys(string input)
    {
        var result = new Dictionary<string, IMonkey>();
        
        foreach (var line in input.Lines())
        {
            if (ValueMonkey.GetParser().TryMatch(line, out ValueMonkey vm))
                result[vm.Name] = vm;
            else if (MonkeyExpression.GetParser().TryMatch(line, out MonkeyExpression me))
                result[me.Name] = new ExpressionMonkey() { Expression = me };
        }
        foreach(var em in result.Values.OfType<ExpressionMonkey>())
        {
            em.Left = result[em.Expression.Left];
            em.Right = result[em.Expression.Right];
        }
        return result;
    }

    protected override long? Part1()
    {
        static long GetRootValue(string input) => GetMonkeys(input)[ROOT].CalcValue();
        Assert(GetRootValue(Sample()), 152);
        return GetRootValue(Input);
    }

    protected override long? Part2()
    {
        static long GetHumanValue(string input) => GetMonkeys(input)[ROOT].SolveForHuman(0);
        Assert(GetHumanValue(Sample()), 301);
        return GetHumanValue(Input);
    }
}
