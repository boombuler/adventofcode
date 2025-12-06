namespace AdventOfCode;

using AdventOfCode.Console;

class SolutionExecutor : ScreenBase, IOutput
{
    private int fAssertionCounter = 1;
    private readonly SolutionResults fResults = new();

    #region Console

    private void WriteLn<T>(object output) where T : OutputMode, new()
        => Console<T>().Write(Convert.ToString(output) + Environment.NewLine);

    #endregion

    public async Task Run(ISolution solution)
    {
        try
        {
            WriteLn<DefaultOut>($" ====================");
            WriteLn<DefaultOut>($" = Year {solution.Year:d4} Day {solution.Day:d2} =");
            WriteLn<DefaultOut>($" ====================");
            WriteLn<DefaultOut>(string.Empty);

            WriteLn<DefaultOut>("-- Part 1 --");
            var p1 = await solution.Part1(this);
            WriteLn<DefaultOut>(string.Format("              Solution : {0}", p1));
            WriteLn<DefaultOut>(string.Empty);
            WriteLn<DefaultOut>(string.Empty);

            string? p2 = null;
            if (solution.Day < 25)
            {
                fAssertionCounter = 1;
                WriteLn<DefaultOut>("-- Part 2 --");

                p2 = await solution.Part2(this);
                if (!string.IsNullOrEmpty(p2))
                    WriteLn<DefaultOut>(string.Format("              Solution : {0}", p2));
                WriteLn<DefaultOut>(string.Empty);
            }
            fResults.UpdateResults(solution, p1, p2 ?? string.Empty);
        }
        catch (Exception e)
        {
            WriteLn<ErrorOut>(e.Message);
            WriteLn<ErrorOut>(e.StackTrace ?? string.Empty);
            ExitConsoleMode();
        }
    }

    #region IOutput

    void IOutput.Assertion(string? name, bool result, string errorTxt)
    {
        if (string.IsNullOrEmpty(name))
            name = Convert.ToString(fAssertionCounter++);
        Console<AssertOut>().WriteResult(name, result, errorTxt);
    }

    void IOutput.Debug(object output)
        => WriteLn<DebugOut>(output);

    void IOutput.Error(string output)
        => WriteLn<ErrorOut>(output);

    #endregion
}
