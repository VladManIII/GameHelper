using HookLib.Models;
using System.Globalization;

namespace HookLib.Interfaces;

public interface IKeyboardController
{
    bool IsCapsLock { get; }
    bool IsNumLock { get; }
    bool ScrollLock { get; }

    void DownKey(byte key);
    void UpKey(byte key);
    void PressKey(byte key);

    void SetLanguage(KeyboardLang language);
    CultureInfo GetCurrentLanguage();
}
