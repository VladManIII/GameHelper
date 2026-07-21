using HookLib.Implementations;
using HookLib.Models;

namespace HookLib.Helpers;

public static class VkKeyHelper
{
    #region Microsoft Imports

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern short VkKeyScan(char ch);

    #endregion

    public static Keys ToVirtualKey(this char ch)
    {
        short vkey = VkKeyScan(ch);
        if (vkey == -1) return Keys.None;

        Keys retval = (Keys)(vkey & 0xff);
        int modifiers = vkey >> 8;

        if ((modifiers & 1) != 0) retval |= Keys.Shift;
        if ((modifiers & 2) != 0) retval |= Keys.Control;
        if ((modifiers & 4) != 0) retval |= Keys.Alt;

        return retval;
    }

    public static SpecialChar ToSpecialChar(this char ch)
    {
        return ch.ToVirtualKey().ToSpecialChar();
    }

    public static SpecialChar ToSpecialChar(this Keys value)
    {
        return new SpecialChar(value, value.HasFlag(Keys.Shift), value.HasFlag(Keys.Control), value.HasFlag(Keys.Alt));
    }

    public static Keys FormatWithModifiers(this Keys key)
    {
        var fKey = key;
        var isMod = fKey == Keys.LShiftKey || fKey == Keys.RShiftKey || fKey == Keys.LControlKey || fKey == Keys.RControlKey || fKey == Keys.LMenu || fKey == Keys.RMenu;
        if (isMod) return fKey;
        if (KeyboardController.Instance.IsShift) fKey |= Keys.Shift;
        if (KeyboardController.Instance.IsControl) fKey |= Keys.Control;
        if (KeyboardController.Instance.IsAlt) fKey |= Keys.Alt;

        return fKey;
    }

    public static bool IsEqual(this Keys key1, Keys key2)
    {
        var result = KeyboardController.Instance.HasModifiers ? key1.FormatWithModifiers() == key2 : key1 == key2;
        return result;
    }
}
