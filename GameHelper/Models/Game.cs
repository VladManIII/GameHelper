using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace GameHelper.Models;

public partial class Game : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    public bool isActive { get; set; } = true;
    public ObservableCollection<Macro> Macros { get; set; } = new();
}
