namespace AdventOfCode;

using AdventOfCode.Console;

class ScreenBase
{
    private OutputMode fTextMode = null;

    #region Console

    protected T Console<T>()
        where T : OutputMode, new()
    {
        if (fTextMode is not T)
        {
            fTextMode?.Exit();
            fTextMode = new T();
            fTextMode.Enter();
        }
        return (T)fTextMode;
    }
    protected OutputMode Console(OutputMode mode)
    {
        if (fTextMode != mode)
        {
            fTextMode?.Exit();
            fTextMode = mode;
            fTextMode.Enter();
        }
        return mode;
    }

    protected void ExitConsoleMode()
    {
        fTextMode?.Exit();
        fTextMode = null;
    }

    #endregion
}
