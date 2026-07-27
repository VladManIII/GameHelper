using GameHelper.Models;

namespace GameHelper.Tests;

public class GameTests
{
    [Fact]
    public void IsActive_Changed_RaisesPropertyChanged()
    {
        var game = new Game();
        var raised = new List<string?>();
        game.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        game.IsActive = false;

        Assert.Contains(nameof(Game.IsActive), raised);
        Assert.False(game.IsActive);
    }

    [Fact]
    public void Name_Changed_RaisesPropertyChanged()
    {
        var game = new Game();
        var raised = new List<string?>();
        game.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        game.Name = "Valorant";

        Assert.Contains(nameof(Game.Name), raised);
        Assert.Equal("Valorant", game.Name);
    }
}
