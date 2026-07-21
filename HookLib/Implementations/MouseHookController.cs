using HookLib.Helpers;
using HookLib.Interfaces;
using HookLib.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HookLib.Implementations;

internal class MouseHookController : IMouseHookController
{
    #region Microsoft Imports

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    #endregion

    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    private IntPtr _hookId = IntPtr.Zero;
    private Func<MouseValue, MouseAction, bool>? _callback;
    private static LowLevelMouseProc? _hookProc;
    public static MouseHookController Instance => Sync.Instance;

    private class Sync
    {
        static Sync() { }

        internal static readonly MouseHookController Instance = new MouseHookController();
    }

    private MouseHookController() { }

    //--------------------------------------------------------------------------------------------------

    private IntPtr LeftDownParam => (IntPtr)VirtualKeyCodes.WM_LBUTTONDOWN;
    private IntPtr LeftUpParam => (IntPtr)VirtualKeyCodes.WM_LBUTTONUP;
    private IntPtr RightDownParam => (IntPtr)VirtualKeyCodes.WM_RBUTTONDOWN;
    private IntPtr RightUpParam => (IntPtr)VirtualKeyCodes.WM_RBUTTONUP;
    private IntPtr MoveParam => (IntPtr)VirtualKeyCodes.WM_MOUSEMOVE;
    private IntPtr WheelParam => (IntPtr)VirtualKeyCodes.WM_MOUSEWHEEL;

    //--------------------------------------------------------------------------------------------------

    public event Action<MouseValue, MouseAction>? KeyChanged;

    //--------------------------------------------------------------------------------------------------

    // if hookCallback return true then do nothing if false send pressed key to system
    public IntPtr SetHook(Func<MouseValue, MouseAction, bool> hookCallback)
    {
        ClearAll();
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            _callback = hookCallback;
            _hookProc = new LowLevelMouseProc(HookCallback);
            _hookId = SetWindowsHookEx(VirtualKeyCodes.WH_MOUSE_LL, _hookProc, GetModuleHandle(curModule.ModuleName), 0);
            if (_hookId == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"Failed to install low-level mouse hook (Win32 error {error}).");
            }
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

        var mhs = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

        if (wParam == LeftDownParam)
            return OnKeyChanged(new MouseValue(new Point(mhs.pt.X, mhs.pt.Y), (int)mhs.mouseData), MouseAction.LeftDown, nCode, wParam, lParam);
        else if (wParam == LeftUpParam)
            return OnKeyChanged(new MouseValue(new Point(mhs.pt.X, mhs.pt.Y), (int)mhs.mouseData), MouseAction.LeftUp, nCode, wParam, lParam);
        else if (wParam == RightDownParam)
            return OnKeyChanged(new MouseValue(new Point(mhs.pt.X, mhs.pt.Y), (int)mhs.mouseData), MouseAction.RightDown, nCode, wParam, lParam);
        else if (wParam == RightUpParam)
            return OnKeyChanged(new MouseValue(new Point(mhs.pt.X, mhs.pt.Y), (int)mhs.mouseData), MouseAction.RightUp, nCode, wParam, lParam);
        else if (wParam == MoveParam)
            return OnKeyChanged(new MouseValue(new Point(mhs.pt.X, mhs.pt.Y), (int)mhs.mouseData), MouseAction.Moved, nCode, wParam, lParam);
        else if (wParam == WheelParam)
            return OnKeyChanged(new MouseValue(new Point(mhs.pt.X, mhs.pt.Y), (int)mhs.mouseData), MouseAction.Wheel, nCode, wParam, lParam);

        return OnKeyChanged(new MouseValue(new Point(mhs.pt.X, mhs.pt.Y), (int)mhs.mouseData), MouseAction.Unknown, nCode, wParam, lParam);
    }

    private IntPtr OnKeyChanged(MouseValue mv, MouseAction ma, int nCode, IntPtr wParam, IntPtr lParam)
    {
        KeyChanged?.Invoke(mv, ma);
        if (_callback == null) return CallNextHookEx(_hookId, nCode, wParam, lParam);
        return _callback.Invoke(mv, ma) ? (IntPtr)1 : CallNextHookEx(_hookId, nCode, wParam, lParam);
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
        KeyChanged = null;
    }

    //-------------------------------------------------------------------------------------------------

    [StructLayout(LayoutKind.Sequential)]
    private class POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private class MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public uint dwExtraInfo;
    }
}
