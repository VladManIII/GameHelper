using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HookLib;
using HookLib.Models;
using System.Windows.Forms;

namespace GameHelper.Models;

public partial class KeyBind : ObservableObject
{
    public KeyBind() { }

    public KeyBind(Keys key)
    {
        Key = key;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    public partial Keys Key { get; set; } = Keys.None;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    public partial bool IsListening { get; set; }

    public string DisplayName => IsListening ? "Press any key…" : Key == Keys.None ? "Not set" : Key.ToString();

    [RelayCommand]
    private void Rebind()
    {
        if (IsListening) return;

        IsListening = true;
        Controllers.KeyboardHookController.KeyChanged += OnKeyCaptured;
    }

    private void OnKeyCaptured(Keys key, KeysState state)
    {
        if (state != KeysState.Down) return;

        Controllers.KeyboardHookController.KeyChanged -= OnKeyCaptured;
        Key = key;
        IsListening = false;
    }
}
