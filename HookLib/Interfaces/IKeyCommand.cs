namespace HookLib.Interfaces;

public interface IKeyCommand
{
    string Key { get; }
    Keys KeysKey { get; }
    string Value { get; }

    void Execute(CancellationToken token = default);
    void ExecuteWith(Keys beforeExecutingKey = Keys.None,
                     Keys afterExecutingKey = Keys.None,
                     int intervalBofreExecute = 0,
                     int intervalAfterExecute = 0,
                     int keyPressInterval = 0,
                     bool randomize = false,
                     CancellationToken token = default);
}
