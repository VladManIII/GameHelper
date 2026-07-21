using HookLib.Helpers;
using HookLib.Interfaces;
using HookLib.Models;
using System.Globalization;
using System.Runtime.InteropServices;

namespace HookLib.Implementations;

internal class KeyboardController : IKeyboardController
{
    #region Microsoft Imports

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, UInt32 dwFlags, uint dwExtraInfo);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern short GetKeyState(int keyCode);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    private static extern int LoadKeyboardLayout(string pwszKLID, uint Flags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetKeyboardLayout(uint thread);

    //[DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);

    #endregion

    public static KeyboardController Instance => Sync.Instance;

    private class Sync
    {
        static Sync() { }

        internal static readonly KeyboardController Instance = new KeyboardController();
    }

    private KeyboardController() { }

    public bool IsCapsLock => (((ushort)GetKeyState((int)Keys.CapsLock)) & 0xffff) != 0;

    public bool IsNumLock => (((ushort)GetKeyState((int)Keys.NumLock)) & 0xffff) != 0;

    public bool ScrollLock => (((ushort)GetKeyState((int)Keys.Scroll)) & 0xffff) != 0;


    public bool IsShift { get; internal set; } = false;
    public bool IsControl { get; internal set; } = false;
    public bool IsAlt { get; internal set; } = false;

    public bool HasModifiers => IsShift || IsControl || IsAlt;


    //--------------------------------------------------------------------------------------------------------
    public void DownKey(byte key)
    {
        keybd_event(key, 0x45, VirtualKeyCodes.KeyEvent | 0, 0);
    }

    public void UpKey(byte key)
    {
        keybd_event(key, 0x45, VirtualKeyCodes.KeyEvent | VirtualKeyCodes.KeyEventUp, 0);
    }

    public void PressKey(byte key)
    {
        keybd_event(key, 0x45, VirtualKeyCodes.KeyEvent | 0, 0);
        keybd_event(key, 0x45, VirtualKeyCodes.KeyEvent | VirtualKeyCodes.KeyEventUp, 0);
    }

    public void DownKey(Keys key)
    {
        DownKey((byte)key);
    }

    public void UpKey(Keys key)
    {
        UpKey((byte)key);
    }

    public void PressKey(Keys key)
    {
        PressKey((byte)key);
    }

    public void PressKey(SpecialChar sp)
    {
        if (sp == null) return;

        if (sp.IsShift) DownKey(Keys.ShiftKey);
        if (sp.IsControl) DownKey(Keys.ControlKey);
        if (sp.IsAlt) DownKey(Keys.LMenu);

        PressKey(sp.Key);

        if (sp.IsShift) UpKey(Keys.ShiftKey);
        if (sp.IsControl) UpKey(Keys.ControlKey);
        if (sp.IsAlt) UpKey(Keys.LMenu);
    }

    public void DownKey(SpecialChar sp)
    {
        if (sp == null) return;

        if (sp.IsShift) DownKey(Keys.ShiftKey);
        if (sp.IsControl) UpKey(Keys.ControlKey);
        if (sp.IsAlt) UpKey(Keys.LMenu);
        //if (sp.IsShift) DownKey(Keys.ShiftKey);
        //if (sp.IsControl) DownKey(Keys.ControlKey);
        //if (sp.IsAlt) DownKey(Keys.LMenu);

        DownKey(sp.Key);
    }

    public void UpKey(SpecialChar sp)
    {
        if (sp == null) return;

        UpKey(sp.Key);

        if (sp.IsShift) UpKey(Keys.ShiftKey);
        if (sp.IsControl) UpKey(Keys.ControlKey);
        if (sp.IsAlt) UpKey(Keys.LMenu);
    }

    public void ReleaseModifiers()
    {
        if (IsShift)
        {
            UpKey(Keys.LShiftKey);
            UpKey(Keys.RShiftKey);
            IsShift = false;
        }
        if (IsControl)
        {
            UpKey(Keys.LControlKey);
            UpKey(Keys.RControlKey);
            IsControl = false;
        }
        if (IsAlt)
        {
            UpKey(Keys.LMenu);
            UpKey(Keys.RMenu);
            IsAlt = false;
        }
    }

    public void SetLanguage(KeyboardLang language)
    {
        SetLanguage(language.Value);
    }

    private void SetLanguage(string language)
    {
        try
        {
            int ret = LoadKeyboardLayout(language, 1);
            PostMessage(GetForegroundWindow(), 0x50, 1, ret);
        }
        catch { }
    }

    public CultureInfo GetCurrentLanguage()
    {
        try
        {
            //IntPtr foregroundWindow = GetForegroundWindow();
            //uint foregroundProcess = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
            //int keyboardLayout = GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF;
            int keyboardLayout = GetKeyboardLayout(0).ToInt32() & 0xFFFF;
            return new CultureInfo(keyboardLayout);
        }
        catch
        {
            return new CultureInfo(1033); // Assume English if something went wrong.
        }
    }
}
