using System.Collections.ObjectModel;

namespace GameHelper.Models;

public class Game
{
    public string Name { get; set; } = string.Empty;
    public bool isActive { get; set; } = true;
    public ObservableCollection<Macro> Macros { get; set; } = new();
}
