using HookLib.Models;

namespace HookLib.Interfaces;

public interface IMouseHookController : IDisposable
{
    event Action<MouseValue, MouseAction>? KeyChanged;
    IntPtr SetHook(Func<MouseValue, MouseAction, bool> hookCallback);

    void Unhook();
}
