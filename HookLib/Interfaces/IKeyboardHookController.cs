using HookLib.Models;

namespace HookLib.Interfaces;

public interface IKeyboardHookController : IDisposable
{
    event Action<Keys, KeysState>? KeyChanged;
    IntPtr SetHook(Func<Keys, KeysState, bool> hookCallback);
    void Unhook();
}
