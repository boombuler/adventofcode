namespace AdventOfCode._2025;

using static Parser;

class Day10 : Solution
{
    record Machine(int[] Lights, int[][] Buttons, int[] Joltages);

    private static readonly Parser<Machine[]> InputParser = (
        from l in "[" + AnyChar(".#", [0, 1]).Many() + "]"
        from btn in ("(" + Int.List(',') + ")").Token().Many()
        from j in "{" + Int.List(',') + "}"
        select new Machine(l, btn, j)).List('\n');

    private long Solve(string input) => (
        from m in InputParser.MustParse(input)
        let result = m.Lights.Reverse().Aggregate(0L, (a, b) => (a << 1) + b)
        let buttons = m.Buttons.Select(btn => btn.Aggregate(0L, (a, b) => a | (1L << b))).ToList()
        select (
            from count in Enumerable.Range(1, buttons.Count - 1)
            where buttons.Combinations(count).Any(c => c.Aggregate((a, b) => a ^ b) == result)
            select count
        ).First()
    ).Sum();

    protected override long? Part1()
    {
        Assert(Solve(Sample()), 7);
        return Solve(Input);
    }

    record struct Equation(int[] Coefficients, int Constant);
    
    private static int SolveMachine(Machine m)
    {
        var constants = m.Joltages.ToArray();
        int[][] coeffs = [.. Enumerable.Range(0, constants.Length).Select(i => m.Buttons.Select(b => b.Contains(i) ? 1 : 0).ToArray())];
        MathExt.GaussElimination(coeffs, constants);
        var equations = constants.Select((c, i) => new Equation(coeffs[i], c))
            .Where(eq => eq.Coefficients.Any(coef => coef != 0))
            .ToList();

        var pivotCols = equations
            .Select(eq => eq.Coefficients.Index().First(i => i.Item != 0).Index)
            .ToHashSet();

        var freeButtons = Enumerable.Range(0, m.Buttons.Length)
            .Where(i => !pivotCols.Contains(i))
            .Select(i => (Index: i, MaxClicks: m.Buttons[i].Min(j => m.Joltages[j])))
            .OrderBy(b => b.MaxClicks)
            .ToArray();

        bool TryDeduceValues(Span<int> values, out int addedSum)
        {
            addedSum = 0;
            bool changed = true;

            while (changed)
            {
                changed = false;

                foreach (var eq in equations)
                {
                    int unknownCol = -1;
                    int unknownCount = 0;

                    for (int col = 0; col < eq.Coefficients.Length; col++)
                    {
                        if (eq.Coefficients[col] != 0 && values[col] == -1)
                        {
                            if (++unknownCount > 1)
                                break;
                            unknownCol = col;
                        }
                    }

                    if (unknownCount != 1) 
                        continue;

                    int sum = eq.Constant;
                    for (int col = 0; col < eq.Coefficients.Length; col++)
                    {
                        if (col != unknownCol && eq.Coefficients[col] != 0)
                            sum -= eq.Coefficients[col] * values[col];
                    }

                    (var val, var rem) = Math.DivRem(sum, eq.Coefficients[unknownCol]);

                    if (rem != 0 || val < 0)
                        return false;

                    values[unknownCol] = val;
                    addedSum += val;
                    changed = true;
                }
            }

            return true;
        }

        bool ValidateSolution(ReadOnlySpan<int> values)
        {  
            foreach(var eq in equations) 
            {
                int sum = 0;
                for (int col = 0; col < eq.Coefficients.Length; col++)
                    sum += eq.Coefficients[col] * values[col];
                if (sum != eq.Constant)
                    return false;
            }
            return true;
        }

        int minResult = int.MaxValue;
        void TryFreeButtons(Span<int> values, int freeIdx, int curSum)
        {
            if (curSum >= minResult) 
                return;

            Span<int> copy = stackalloc int[values.Length];
            values.CopyTo(copy);
            if (!TryDeduceValues(copy, out int added))
                return;

            if (freeIdx >= freeButtons.Length)
            {
                curSum += added;
                if (curSum < minResult && ValidateSolution(copy))
                    minResult = curSum;
            }
            else
            {
                var btn = freeButtons[freeIdx];
                for (int v = 0; v <= btn.MaxClicks; v++)
                {
                    values[btn.Index] = v;
                    TryFreeButtons(values, freeIdx + 1, curSum + v);
                }
                values[btn.Index] = -1;
            }
        }

        Span<int> buttonValues = stackalloc int[m.Buttons.Length];
        buttonValues.Fill(-1);
        TryFreeButtons(buttonValues, 0, 0);
        
        return minResult;
    }

    protected override long? Part2()
    {
        static long Solve(string input)
            => InputParser.MustParse(input).AsParallel().Sum(SolveMachine);
        
        Assert(Solve(Sample()), 33);
        return Solve(Input);
    }
}
