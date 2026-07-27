using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

using GameHelper.Models;
using GameHelper.Services.Persistence;

namespace GameHelper.Services;

// Owns the Games collection shared between the UI (MainViewModel) and the macro
// dispatcher, loading it from PersistenceService at startup (or seeding defaults on
// first run) and writing it back out on demand.
public class GameLibraryService
{
    private readonly PersistenceService _persistence;

    public ObservableCollection<Game> Games { get; }

    public GameLibraryService(PersistenceService persistence)
    {
        _persistence = persistence;

        var records = persistence.LoadGames();
        Games = records.Count > 0
            ? new ObservableCollection<Game>(records.Select(ToGame))
            : SeedDefaults();
    }

    public void Save() => _persistence.SaveGames(Games.Select(ToRecord));

    private static ObservableCollection<Game> SeedDefaults() => new()
    {
        new Game
        {
            Name = "PUBG",
            Macros = new ObservableCollection<Macro>
            {
                new Macro { Name = "Window Jump", KeyBind = new KeyBind(Keys.Space), Sequence = " c" },
            },
        },
        new Game
        {
            Name = "Rust",
            Macros = new ObservableCollection<Macro>
            {
                new Macro { Name = "Home TP", KeyBind = new KeyBind(Keys.NumPad1), Sequence = "t /home 1" },
                new Macro { Name = "Accept TP", KeyBind = new KeyBind(Keys.NumPad2), Sequence = "t /tpa" },
                new Macro { Name = "Cancel TP", KeyBind = new KeyBind(Keys.NumPad3), Sequence = "t /tpc" },
            },
        },
    };

    private static Game ToGame(GameRecord record) => new()
    {
        Name = record.Name,
        IsActive = record.IsActive,
        Macros = new ObservableCollection<Macro>(record.Macros.Select(m => new Macro
        {
            Name = m.Name,
            Sequence = m.Sequence,
            KeyBind = new KeyBind(m.Key),
        })),
    };

    private static GameRecord ToRecord(Game game) => new()
    {
        Name = game.Name,
        IsActive = game.IsActive,
        Macros = game.Macros.Select(m => new MacroRecord
        {
            Name = m.Name,
            Sequence = m.Sequence,
            Key = m.KeyBind.Key,
        }).ToList(),
    };
}
