﻿namespace AdventOfCode;

using AdventOfCode.Console;

internal class Validator : ScreenBase
{
    class Default : OutputMode
    {
        public override void Enter()
        {
            SetBG(DEFAULT_BACKGROUND);
            SetFG(DEFAULT_FOREGROUND);
        }
        public override void Exit()
        {
        }
    }
    class ValidateOut : OutputMode
    {
        public override void Enter() => SetBG(DEFAULT_BACKGROUND);

        public void Write(bool ok)
        {
            if (ok)
            {
                SetFG(0x009900);
                base.Write("✓");
            }
            else
            {
                SetFG(COLOR_CRIMSON);
                base.Write("⨯");
            }
        }
    }
    class Output : IOutput
    {
        public bool Failed { get; private set; }
        void IOutput.Assertion(string name, bool result, string errorTxt)
            => Failed |= !result;

        void IOutput.Debug(object data) { }

        void IOutput.Error(string data) => Failed = true;
    }

    private readonly SolutionResults fResults = new();

    private const int RESULT_WIDTH = 25 + 2;

    public async Task Run(IEnumerable<ISolution> solutions)
    {
        var years = solutions.GroupBy(s => s.Year).OrderBy(g => g.Key);

        var columns = System.Console.BufferWidth / RESULT_WIDTH;
        var col = 0;

        foreach (var year in years)
        {
            if (col == 0)
            {
                Console<Default>().WriteLine("");
                Console<Default>().WriteLine("");
                Console<Default>().WriteLine("");
                Console<Default>().WriteLine("");
            }

            await RunYear(year.Key, year, col);
            col = (col + 1) % columns;
        }
    }

    private async Task RunYear(int year, IEnumerable<ISolution> solutions, int column)
    {
        var (x, y) = System.Console.GetCursorPosition();
        y -= 4;
        x = column * RESULT_WIDTH;
        System.Console.SetCursorPosition(x, y++);

        Console<Default>().Write($"> {year:d4} < 1111111111222222");
        System.Console.SetCursorPosition(x, y++);
        Console<Default>().Write("1234567890123456789012345");
        System.Console.SetCursorPosition(x, y++);
        int day = 1;

        foreach (var solution in solutions.OrderBy(s => s.Day))
        {
            var offset = solution.Day - day++;
            Console<ValidateOut>().Write(new string(' ', offset));
            Console<ValidateOut>().Write(await Validate(solution));
        }
        Console<Default>().WriteLine("");
    }

    private async Task<bool> Validate(ISolution solution)
    {
        try
        {
            var output = new Output();
            var (res1, res2) = fResults.GetResults(solution);

            var p1 = await solution.Part1(output);
            if (output.Failed || (!string.IsNullOrEmpty(res1) && Convert.ToString(p1) != res1))
                return false;
            if (solution.Day < 25)
            {
                var p2 = await solution.Part2(output);
                if (output.Failed | (!string.IsNullOrEmpty(res2) && Convert.ToString(p2) != res2))
                    return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
