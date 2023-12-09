namespace AdventOfCode.Console;

class AttributeMode(int color) : OutputMode
{
    const int COLOR_SELECTED = 0x25243E;
    private bool fSelected;
    private readonly int fColor = color;
    public bool Selected
    {
        get => fSelected;
        set
        {
            if (fSelected != value)
            {
                SetBG(value ? COLOR_SELECTED : DEFAULT_BACKGROUND);
                fSelected = value;
            }
        }
    }

    public override void Enter()
    {
        SetBG(fSelected ? COLOR_SELECTED : DEFAULT_BACKGROUND);
        SetFG(fColor);
    }

    public override void Exit()
    {
    }
}
