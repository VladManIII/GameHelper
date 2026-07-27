namespace GameHelper.Models;

public class Macro
{
    public string Name { get; set; } = string.Empty;
    public KeyBind KeyBind { get; set; } = new();
    public string Macros { get; set; } = string.Empty;
}
