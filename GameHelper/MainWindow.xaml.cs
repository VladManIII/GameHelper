using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.ObjectModel;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using GameHelper.Pages;
using GameHelper.Models;
using GameHelper.Services;

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

        AppWindow.Resize(new Windows.Graphics.SizeInt32(850, 500));
    }

    public override IViewModelLifecycle? GetViewModel() => VievModel;

    private void nvMain_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        settingsPanel.Visibility = args.IsSettingsSelected ? Visibility.Visible : Visibility.Collapsed;
        gamesPanel.Visibility = args.IsSettingsSelected ? Visibility.Collapsed : Visibility.Visible;
    }

    private void MacroField_GotFocus(object sender, RoutedEventArgs e) => VievModel.BeginFieldEdit();

    private void MacroField_LostFocus(object sender, RoutedEventArgs e) => VievModel.EndFieldEdit();

    private void DeleteMacro_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: Macro macro })
            VievModel.SelectedGame?.Macros.Remove(macro);
    }

    private void DeleteGame_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: Game game })
            VievModel.DeleteGame(game);
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
                new Macro { Name = "Window Jump", KeyBind = new KeyBind(Keys.Space), Macros=" c" },
            },
        },
        new Game
        {
            Name = "Rust",
            Macros = new ObservableCollection<Macro>
            {
                new Macro { Name = "Home TP", KeyBind = new KeyBind(Keys.NumPad1), Macros="t /home 1" },
                new Macro { Name = "Accept TP", KeyBind = new KeyBind(Keys.NumPad2), Macros="t /tpa" },
                new Macro { Name = "Cancel TP", KeyBind = new KeyBind(Keys.NumPad3), Macros="t /tpc" },
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

    private readonly MacroDispatcherService _macroDispatcher;
    private int _fieldEditCount;

    public MainViewModel()
    {
        SelectedGame = Games.FirstOrDefault();
        _macroDispatcher = new MacroDispatcherService(Games);
    }

    // Tracked with a counter (not a bool) so moving focus directly between two macro
    // fields doesn't briefly un-pause dispatch between the old field's LostFocus and
    // the new field's GotFocus.
    public void BeginFieldEdit()
    {
        _fieldEditCount++;
        _macroDispatcher.IsPaused = true;
    }

    public void EndFieldEdit()
    {
        _fieldEditCount = Math.Max(0, _fieldEditCount - 1);
        _macroDispatcher.IsPaused = _fieldEditCount > 0;
    }

    [RelayCommand]
    private void AddGame()
    {
        var game = new Game { Name = $"New Game {Games.Count + 1}" };
        Games.Add(game);
        SelectedGame = game;
    }

    [RelayCommand(CanExecute = nameof(CanAddMacro))]
    private void AddMacro()
    {
        if (SelectedGame == null) return;

        SelectedGame.Macros.Add(new Macro { Name = $"New Macro {SelectedGame.Macros.Count + 1}" });
    }

    private bool CanAddMacro() => SelectedGame != null;

    public void DeleteGame(Game game)
    {
        var index = Games.IndexOf(game);
        if (index < 0) return;

        Games.Remove(game);

        if (SelectedGame == game)
            SelectedGame = Games.Count > 0 ? Games[Math.Min(index, Games.Count - 1)] : null;
    }

    partial void OnSelectedGameChanged(Game? value)
    {
        AddMacroCommand.NotifyCanExecuteChanged();
    }

    partial void OnKeyHooksEnabledChanged(bool value)
    {
        if (value)
            _macroDispatcher.Start();
        else
            _macroDispatcher.Stop();
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        if (KeyHooksEnabled)
            _macroDispatcher.Start();

        Status = "Ready on Activate";
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        _macroDispatcher.Stop();
    }

    protected override void OnVisibilityChanged()
    {
        base.OnVisibilityChanged();
        // Handle visibility change logic here
    }
}
