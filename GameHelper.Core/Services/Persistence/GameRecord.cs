using System.Collections.Generic;
using System.Windows.Forms;

namespace GameHelper.Services.Persistence;

// Plain storage shape for a Game, kept separate from the UI-facing GameHelper.Models.Game
// (an ObservableObject with source-generated RelayCommand properties) so LiteDB never has
// to serialize ICommand instances.
public class GameRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<MacroRecord> Macros { get; set; } = new();
}

public class MacroRecord
{
    public string Name { get; set; } = string.Empty;
    public string Sequence { get; set; } = string.Empty;
    public Keys Key { get; set; }
}
