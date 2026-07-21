using HookLib.Helpers;
using HookLib.Interfaces;
using HookLib.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HookLib.Implementations;

public class KeyboardHookController : IKeyboardHookController
{
    #region Microsoft Imports

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    #endregion

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private IntPtr _hookId = IntPtr.Zero;
    private Func<Keys, KeysState, bool>? _callback;
    private static LowLevelKeyboardProc? _hookProc;
    public static KeyboardHookController Instance => Sync.Instance;

    private class Sync
    {
        static Sync() { }

        internal static readonly KeyboardHookController Instance = new KeyboardHookController();
    }

    private KeyboardHookController() { }

    //--------------------------------------------------------------------------------------------------

    private IntPtr KeyDownParam => (IntPtr)VirtualKeyCodes.WM_KEYDOWN;
    private IntPtr SysKeyDownParam => (IntPtr)VirtualKeyCodes.WM_SYSKEYDOWN;

    //--------------------------------------------------------------------------------------------------

    public event Action<Keys, KeysState>? KeyChanged;

    //--------------------------------------------------------------------------------------------------

    // if hookCallback return true then do nothing if false send pressed key to system
    public IntPtr SetHook(Func<Keys, KeysState, bool> hookCallback)
    {
        ClearAll();
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            _callback = hookCallback;
            _hookProc = new LowLevelKeyboardProc(HookCallback);
            _hookId = SetWindowsHookEx(VirtualKeyCodes.WH_KEYBOARD_LL, _hookProc, GetModuleHandle(curModule.ModuleName), 0);
            return _hookId;
        }
    }

    public void Unhook()
    {
        if (_hookId != IntPtr.Zero) UnhookWindowsHookEx(_hookId);
    }

    //--------------------------------------------------------------------------------------------------

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0) return CallNextHookEx(_hookId, nCode, wParam, lParam);
        var key = (Keys)Marshal.ReadInt32(lParam);

        if ((wParam == KeyDownParam) || (wParam == SysKeyDownParam))
            return OnKeyChanged(key, KeysState.Down, nCode, wParam, lParam);

        return OnKeyChanged(key, KeysState.Up, nCode, wParam, lParam);
    }

    private IntPtr OnKeyChanged(Keys key, KeysState state, int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (key == Keys.LShiftKey || key == Keys.RShiftKey) KeyboardController.Instance.IsShift = state == KeysState.Down ? true : false;
        else if (key == Keys.LControlKey || key == Keys.RControlKey) KeyboardController.Instance.IsControl = state == KeysState.Down ? true : false;
        else if (key == Keys.LMenu || key == Keys.RMenu) KeyboardController.Instance.IsAlt = state == KeysState.Down ? true : false;

        key = key.FormatWithModifyers();

        KeyChanged?.Invoke(key, state);
        if (_callback == null) return CallNextHookEx(_hookId, nCode, wParam, lParam);
        return _callback.Invoke(key, state) ? (IntPtr)1 : CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private void ClearAll()
    {
        try
        {
            Unhook();
            if (_callback != null)
                _callback = null;
            if (_hookProc != null)
                _hookProc = null;

            _hookId = IntPtr.Zero;
        }
        catch { }
    }

    public void Dispose()
    {
        ClearAll();
    }
}
