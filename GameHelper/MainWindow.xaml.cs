using CommunityToolkit.Mvvm.ComponentModel;
using GameHelper.Models;
using GameHelper.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameHelper;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : BaseWindow
{
    public MainViewModel VievModel { get; set; } = new();

    public MainWindow()
    {
        InitializeComponent();

        AppWindow.Resize(new Windows.Graphics.SizeInt32(700, 500));
    }

    public override IViewModelLifecycle? GetViewModel() => VievModel;

    private void nvMain_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        settingsPanel.Visibility = args.IsSettingsSelected ? Visibility.Visible : Visibility.Collapsed;
        gamesPanel.Visibility = args.IsSettingsSelected ? Visibility.Collapsed : Visibility.Visible;
    }
}

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<Game> Games { get; } = new()
    {
        new Game
        {
            Name = "PUBG",
            Macros = new ObservableCollection<Macro>
            {
                new Macro { Name = "Window Jump", KeyBind = "Space", Macros="Space+C" },
            },
        },
        new Game
        {
            Name = "Rust",
            Macros = new ObservableCollection<Macro>
            {
                new Macro { Name = "Home TP", KeyBind = "NumPad1", Macros="/home 1" },
                new Macro { Name = "Accept TP", KeyBind = "NumPad2", Macros="/tpa" },
                new Macro { Name = "Cancel TP", KeyBind = "NumPad3", Macros="/tpc" },
            },
        },
    };

    [ObservableProperty]
    public partial Game? SelectedGame { get; set; }

    [ObservableProperty]
    public partial bool RunOnStartup { get; set; }

    [ObservableProperty]
    public partial bool KeyHooksEnabled { get; set; } = true;

    [ObservableProperty]
    public partial string Status {  get; set; }

    public MainViewModel()
    {
        SelectedGame = Games.FirstOrDefault();
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        Status = "Ready on Activate";
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        // Handle cleanup logic here
    }

    protected override void OnVisibilityChanged()
    {
        base.OnVisibilityChanged();
        // Handle visibility change logic here
    }
}
