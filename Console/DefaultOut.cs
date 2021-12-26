namespace AdventOfCode.Console;

class DefaultOut : OutputMode
{
    public override void Enter()
    {
        SetBG(DEFAULT_BACKGROUND);
        SetFG(DEFAULT_FOREGROUND);
        base.Enter();
    }
}
