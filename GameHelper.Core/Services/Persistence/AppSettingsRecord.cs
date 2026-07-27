namespace GameHelper.Services.Persistence;

public class AppSettingsRecord
{
    public int Id { get; set; } = 1;
    public bool RunOnStartup { get; set; }
    public bool KeyHooksEnabled { get; set; } = true;
}
