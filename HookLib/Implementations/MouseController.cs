using HookLib.Helpers;
using HookLib.Interfaces;
using System.Runtime.InteropServices;

namespace HookLib.Implementations;

internal class MouseController : IMouseController
{
    #region Microsoft Imports

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    #endregion

    public static MouseController Instance => Sync.Instance;

    private class Sync
    {
        static Sync() { }

        internal static readonly MouseController Instance = new MouseController();
    }

    private MouseController() { }

    //--------------------------------------------------------------------------------------------------------

    public void DownLeftKey()
    {
        uint X = (uint)Cursor.Position.X;
        uint Y = (uint)Cursor.Position.Y;

        mouse_event(VirtualKeyCodes.MOUSEEVENTF_LEFTDOWN | 0, X, Y, 0, 0);
    }

    public void UpLeftKey()
    {
        uint X = (uint)Cursor.Position.X;
        uint Y = (uint)Cursor.Position.Y;

        mouse_event(VirtualKeyCodes.MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
    }

    public void PressLeftKey()
    {
        uint X = (uint)Cursor.Position.X;
        uint Y = (uint)Cursor.Position.Y;

        mouse_event(VirtualKeyCodes.MOUSEEVENTF_LEFTDOWN | 0, X, Y, 0, 0);
        mouse_event(VirtualKeyCodes.MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
    }

    public void PressLeftKey(int x, int y)
    {
        Cursor.Position = new Point(x, y);

        mouse_event(VirtualKeyCodes.MOUSEEVENTF_LEFTDOWN | VirtualKeyCodes.MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
    }

    public void DownRightKey()
    {
        uint X = (uint)Cursor.Position.X;
        uint Y = (uint)Cursor.Position.Y;

        mouse_event(VirtualKeyCodes.MOUSEEVENTF_RIGHTDOWN | 0, X, Y, 0, 0);
    }

    public void UpRightKey()
    {
        uint X = (uint)Cursor.Position.X;
        uint Y = (uint)Cursor.Position.Y;

        mouse_event(VirtualKeyCodes.MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
    }

    public void PressRightKey()
    {
        uint X = (uint)Cursor.Position.X;
        uint Y = (uint)Cursor.Position.Y;

        mouse_event(VirtualKeyCodes.MOUSEEVENTF_RIGHTDOWN | 0, X, Y, 0, 0);
        mouse_event(VirtualKeyCodes.MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
    }

    public void PressRightKey(int x, int y)
    {
        Cursor.Position = new Point(x, y);

        mouse_event(VirtualKeyCodes.MOUSEEVENTF_RIGHTDOWN | VirtualKeyCodes.MOUSEEVENTF_RIGHTUP, (uint)x, (uint)y, 0, 0);
    }

    public void MoveTo(int x, int y, bool emulateReal = false)
    {
        Cursor.Position = new Point(x, y);
    }
}
