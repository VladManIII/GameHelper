using HookLib.Interfaces;

namespace HookLib;

public static class Controllers
{
    public static IKeyboardController KeyboardController => Implementations.KeyboardController.Instance;

    public static IMouseController MouseController => Implementations.MouseController.Instance;

    public static IMouseHookController MouseHookController => Implementations.MouseHookController.Instance;

    public static IKeyboardHookController KeyboardHookController => Implementations.KeyboardHookController.Instance;

    public static void UnhookAll()
    {
        MouseHookController.Unhook();
        KeyboardHookController.Unhook();
    }
}
