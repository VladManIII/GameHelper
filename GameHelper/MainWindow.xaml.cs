using System;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.UI.Xaml.Controls;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

using GameHelper.Pages;
using GameHelper.Models;
using GameHelper.Services;
using GameHelper.Services.Persistence;
using GameHelper.Views;

namespace GameHelper;

public sealed partial class MainWindow : BaseWindow
{
    public MainViewModel VievModel { get; }

    public MainWindow(MainViewModel viewModel)
    {
        VievModel = viewModel;

        InitializeComponent();
        AppWindow.Resize(new Windows.Graphics.SizeInt32(850, 500));

        contentFrame.Navigate(typeof(GamesPage), VievModel);
    }

    public override IViewModelLifecycle? GetViewModel() => VievModel;

    private void nvMain_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        contentFrame.Navigate(args.IsSettingsSelected ? typeof(SettingsPage) : typeof(GamesPage), VievModel);
    }
}

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<Game> Games { get; }

    [ObservableProperty] public partial Game? SelectedGame { get; set; }
    [ObservableProperty] public partial bool RunOnStartup { get; set; }
    [ObservableProperty] public partial bool KeyHooksEnabled { get; set; } = true;
    [ObservableProperty] public partial string Status { get; set; } = "Idle";

    private readonly GameLibraryService _gameLibrary;
    private readonly MacroDispatcherService _macroDispatcher;
    private readonly PersistenceService _persistence;
    private int _fieldEditCount;
    private bool _hasActivatedOnce;

    public MainViewModel(GameLibraryService gameLibrary, MacroDispatcherService macroDispatcher, PersistenceService persistence)
    {
        _gameLibrary = gameLibrary;
        _macroDispatcher = macroDispatcher;
        _persistence = persistence;

        Games = gameLibrary.Games;
        SelectedGame = Games.FirstOrDefault();

        var settings = persistence.LoadSettings();
        RunOnStartup = settings.RunOnStartup;
        KeyHooksEnabled = settings.KeyHooksEnabled;
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

        _gameLibrary.Save();
    }

    [RelayCommand(CanExecute = nameof(CanAddMacro))]
    private void AddMacro()
    {
        if (SelectedGame == null) return;

        SelectedGame.Macros.Add(new Macro { Name = $"New Macro {SelectedGame.Macros.Count + 1}" });

        _gameLibrary.Save();
    }

    private bool CanAddMacro() => SelectedGame != null;

    [RelayCommand]
    private void DeleteMacro(Macro macro)
    {
        macro.KeyBind.CancelRebind();
        SelectedGame?.Macros.Remove(macro);

        _gameLibrary.Save();
    }

    [RelayCommand]
    private void DeleteGame(Game game)
    {
        var index = Games.IndexOf(game);
        if (index < 0) return;

        foreach (var macro in game.Macros)
            macro.KeyBind.CancelRebind();

        Games.Remove(game);

        if (SelectedGame == game)
            SelectedGame = Games.Count > 0 ? Games[Math.Min(index, Games.Count - 1)] : null;

        _gameLibrary.Save();
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

        _persistence.SaveSettings(new AppSettingsRecord { RunOnStartup = RunOnStartup, KeyHooksEnabled = value });
    }

    partial void OnRunOnStartupChanged(bool value)
    {
        // Persisted, but doesn't yet register/unregister an actual Windows startup entry.
        _persistence.SaveSettings(new AppSettingsRecord { RunOnStartup = value, KeyHooksEnabled = KeyHooksEnabled });
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        // Activated fires on every focus change (e.g. alt-tab back), not just first launch.
        // Only start the dispatcher once here; KeyHooksEnabled owns it after that.
        if (!_hasActivatedOnce)
        {
            _hasActivatedOnce = true;
            if (KeyHooksEnabled)
                _macroDispatcher.Start();
        }

        Status = "Ready";
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        _macroDispatcher.Stop();
        _gameLibrary.Save();
    }

    protected override void OnVisibilityChanged()
    {
        base.OnVisibilityChanged();
        // Handle visibility change logic here
    }
}
