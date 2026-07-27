using System;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using GameHelper.Services;

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

    private IDisposable? _captureSubscription;

    [RelayCommand]
    private void Rebind()
    {
        if (IsListening) return;

        IsListening = true;
        _captureSubscription = KeyCaptureService.ListenOnce(OnKeyCaptured);
    }

    // Must be called when a KeyBind is removed from the model tree (e.g. its owning
    // Macro/Game is deleted) while still listening, otherwise the capture subscription
    // stays alive forever and steals the next global keypress.
    public void CancelRebind()
    {
        if (!IsListening) return;

        _captureSubscription?.Dispose();
        _captureSubscription = null;
        IsListening = false;
    }

    private void OnKeyCaptured(Keys key)
    {
        _captureSubscription = null;
        Key = key;
        IsListening = false;
    }
}
