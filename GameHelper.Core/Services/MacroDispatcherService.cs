using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using HookLib;
using HookLib.Models;
using HookLib.Implementations;

using GameHelper.Models;

namespace GameHelper.Services;

// Listens on the global low-level keyboard hook and, when a pressed key matches a
// macro's KeyBind on an active game, types out that macro's sequence.
public class MacroDispatcherService
{
    private readonly ObservableCollection<Game> _games;
    private readonly SemaphoreSlim _executionGate = new(1, 1);
    private CancellationTokenSource? _executionCts;

    // Suppresses matching/dispatching while true, without tearing down the underlying hook
    // (KeyBind rebind capture keeps working, since it listens on the same hook independently).
    public bool IsPaused { get; set; }

    public MacroDispatcherService(GameLibraryService gameLibrary)
    {
        _games = gameLibrary.Games;
    }

    public void Start()
    {
        Controllers.KeyboardHookController.SetHook(OnKeyEvent);
    }

    public void Stop()
    {
        _executionCts?.Cancel();
        Controllers.KeyboardHookController.Unhook();
    }

    // Returning true swallows the key from reaching the foreground app; false lets it through untouched.
    private bool OnKeyEvent(Keys key, KeysState state)
    {
        if (IsPaused) return false;
        if (state != KeysState.Down) return false;

        var macro = FindMacro(key);
        if (macro == null) return false;

        return TryDispatch(macro);
    }

    private Macro? FindMacro(Keys key)
    {
        foreach (var game in _games)
        {
            if (!game.IsActive) continue;

            foreach (var macro in game.Macros)
            {
                if (macro.KeyBind.Key == key) return macro;
            }
        }

        return null;
    }

    // Only one macro runs at a time; a key press while one is already running is ignored
    // (and passed through) rather than queued or interleaved.
    private bool TryDispatch(Macro macro)
    {
        if (!_executionGate.Wait(0)) return false;

        var cts = new CancellationTokenSource();
        _executionCts = cts;

        Task.Run(() =>
        {
            try
            {
                new KeyCommand(string.Empty, macro.Sequence).Execute(cts.Token);
            }
            finally
            {
                _executionGate.Release();
            }
        });

        return true;
    }
}
