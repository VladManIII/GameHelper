namespace HookLib.Models;

public class SpecialChar
{
    public SpecialChar(Keys key, bool isShift = false, bool isControl = false, bool isAlt = false)
    {
        Key = key;
        IsShift = isShift;
        IsControl = isControl;
        IsAlt = isAlt;
    }
    public Keys Key { get; private set; }
    public bool IsShift { get; private set; }
    public bool IsControl { get; private set; }
    public bool IsAlt { get; private set; }
}
