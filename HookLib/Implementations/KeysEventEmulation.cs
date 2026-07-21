using HookLib.Helpers;

namespace HookLib.Implementations;

internal static class KeysEventEmulation
{
    private static readonly Random GetRandom = new Random();

    public static bool IsExecuting { get; private set; } = false;
    public static void ExecuteSequence(string sequence, int keyPressInterval = 0, bool randomize = false, CancellationToken token = default)
    {
        IsExecuting = true;
        if (token.IsCancellationRequested) return;

        var iscaps = KeyboardController.Instance.IsCapsLock;
        if (iscaps)
            KeyboardController.Instance.PressKey(Keys.CapsLock);

        if (KeyboardController.Instance.IsAlt)
            sequence = sequence[0] + sequence;
        KeyboardController.Instance.ReleaseModifiers();

        for (int i = 0; i < sequence.Length; i++)
        {
            if (token.IsCancellationRequested) return;
            if (keyPressInterval > 0) Thread.Sleep(GetInterval(keyPressInterval, randomize));
            ExecuteKey(sequence[i]);
        }

        if (iscaps)
            KeyboardController.Instance.PressKey(Keys.CapsLock);
        IsExecuting = false;
    }

    public static void ExecuteKey(char value)
    {
        if (char.IsLetterOrDigit(value))
            ExecureLetterOrDigit(value);
        else
            KeyboardController.Instance.PressKey(value.ToSpecialChar());
    }

    private static void ExecureLetterOrDigit(char value)
    {

        bool upperCase = char.IsUpper(value);

        if (upperCase)
            KeyboardController.Instance.DownKey(Keys.ShiftKey);

        KeyboardController.Instance.PressKey((Keys)char.ToUpper(value));

        if (upperCase)
            KeyboardController.Instance.UpKey(Keys.ShiftKey);
    }

    private static int GetInterval(int keyPressInterval, bool randomize)
    {
        if (randomize)
        {
            var diff = (int)Math.Round(keyPressInterval * 0.2);
            return GetRandom.Next(keyPressInterval - diff, keyPressInterval + diff);
        }

        return keyPressInterval;
    }
}
