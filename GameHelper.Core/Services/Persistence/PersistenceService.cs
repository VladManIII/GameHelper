using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using LiteDB;

namespace GameHelper.Services.Persistence;

// Local LiteDB store for games/macros and app settings. One singleton instance for the
// app's lifetime (registered in App.xaml.cs), disposed on shutdown.
public class PersistenceService : IDisposable
{
    private const string GamesCollectionName = "games";
    private const string SettingsCollectionName = "settings";
    private const int SettingsRecordId = 1;

    private readonly LiteDatabase _db;

    public PersistenceService()
    {
        var dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GameHelper");
        Directory.CreateDirectory(dataDir);
        _db = new LiteDatabase(Path.Combine(dataDir, "gamehelper.db"));
    }

    public List<GameRecord> LoadGames() =>
        _db.GetCollection<GameRecord>(GamesCollectionName).FindAll().ToList();

    public void SaveGames(IEnumerable<GameRecord> games)
    {
        var collection = _db.GetCollection<GameRecord>(GamesCollectionName);
        collection.DeleteAll();
        collection.InsertBulk(games);
    }

    public AppSettingsRecord LoadSettings() =>
        _db.GetCollection<AppSettingsRecord>(SettingsCollectionName).FindById(SettingsRecordId)
        ?? new AppSettingsRecord();

    public void SaveSettings(AppSettingsRecord settings)
    {
        settings.Id = SettingsRecordId;
        _db.GetCollection<AppSettingsRecord>(SettingsCollectionName).Upsert(settings);
    }

    public void Dispose() => _db.Dispose();
}
