using HookLib.Models;
using System.Globalization;

namespace HookLib.Interfaces;

public interface IKeyboardController
{
    bool IsCapsLock { get; }
    bool IsNumLock { get; }
    bool ScrollLock { get; }

    bool IsShift { get; }
    bool IsControl { get; }
    bool IsAlt { get; }
    bool HasModifiers { get; }

    void DownKey(byte key);
    void UpKey(byte key);
    void PressKey(byte key);

    void DownKey(Keys key);
    void UpKey(Keys key);
    void PressKey(Keys key);

    void DownKey(SpecialChar sp);
    void UpKey(SpecialChar sp);
    void PressKey(SpecialChar sp);

    void ReleaseModifiers();

    void SetLanguage(KeyboardLang language);
    CultureInfo GetCurrentLanguage();
}
