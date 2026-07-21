using HookLib.Interfaces;

namespace HookLib.Implementations;

public class KeyCommand : IKeyCommand
{
    public KeyCommand(string key, string value)
    {
        Key = key;
        try
        {
            var converted = string.IsNullOrEmpty(key) ? null : new KeysConverter().ConvertFrom(Key);
            KeysKey = converted is Keys keys ? keys : Keys.None;
        }
        catch { KeysKey = Keys.None; }
        Value = value;
    }

    //-----------------------------------------------------------------------------------------------------------

    public string Key { get; private set; }
    public Keys KeysKey { get; private set; }
    public string Value { get; private set; }

    //-----------------------------------------------------------------------------------------------------------

    public void Execute(CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(Value)) return;
        KeysEventEmulation.ExecuteSequence(Value, token: token);
    }

    public void ExecuteWith(Keys beforeExecutingKey = Keys.None,
                            Keys afterExecutingKey = Keys.None,
                            int intervalBofreExecute = 0,
                            int intervalAfterExecute = 0,
                            int keyPressInterval = 0,
                            bool randomize = false,
                            CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(Value)) return;
        ExecuteWithParams(beforeExecutingKey, afterExecutingKey, intervalBofreExecute, intervalAfterExecute, keyPressInterval, randomize, token);
    }

    //-----------------------------------------------------------------------------------------------------------

    private void ExecuteWithParams(Keys beforeExecutingKey = Keys.None,
                                   Keys afterExecutingKey = Keys.None,
                                   int intervalBofreExecute = 0,
                                   int intervalAfterExecute = 0,
                                   int keyPressInterval = 0,
                                   bool randomize = false,
                                   CancellationToken token = default)
    {
        if (beforeExecutingKey != Keys.None) KeyboardController.Instance.PressKey(beforeExecutingKey);
        if (token.IsCancellationRequested) return;
        if (intervalBofreExecute > 0) Thread.Sleep(intervalBofreExecute);
        if (token.IsCancellationRequested) return;
        KeysEventEmulation.ExecuteSequence(Value, keyPressInterval, randomize, token);
        if (token.IsCancellationRequested) return;
        if (intervalAfterExecute > 0) Thread.Sleep(intervalAfterExecute);
        if (token.IsCancellationRequested) return;
        if (afterExecutingKey != Keys.None) KeyboardController.Instance.PressKey(afterExecutingKey);
    }
}
