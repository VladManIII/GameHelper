namespace HookLib.Models;

public class KeyboardLang
{
    private KeyboardLang(string value) { Value = value; }

    public string Value { get; private set; }

    public static KeyboardLang En => new KeyboardLang("00000409");

    public static KeyboardLang Uk => new KeyboardLang("00000422");
}
