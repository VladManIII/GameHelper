using System.Windows.Forms;

using GameHelper.Models;

namespace GameHelper.Tests;

public class KeyBindTests
{
    [Fact]
    public void DisplayName_WhenKeyNotSet_ReturnsNotSet()
    {
        var keyBind = new KeyBind();

        Assert.Equal("Not set", keyBind.DisplayName);
    }

    [Fact]
    public void DisplayName_WhenKeySet_ReturnsKeyName()
    {
        var keyBind = new KeyBind(Keys.Space);

        Assert.Equal("Space", keyBind.DisplayName);
    }

    [Fact]
    public void DisplayName_WhileListening_ReturnsPrompt()
    {
        var keyBind = new KeyBind(Keys.Space);

        keyBind.RebindCommand.Execute(null);
        try
        {
            Assert.True(keyBind.IsListening);
            Assert.Equal("Press any key…", keyBind.DisplayName);
        }
        finally
        {
            keyBind.CancelRebind();
        }
    }

    [Fact]
    public void CancelRebind_StopsListening_AndIsIdempotent()
    {
        var keyBind = new KeyBind(Keys.Space);
        keyBind.RebindCommand.Execute(null);

        keyBind.CancelRebind();
        Assert.False(keyBind.IsListening);

        // Must not throw when called again (e.g. deleting an already-cancelled KeyBind).
        keyBind.CancelRebind();
        Assert.False(keyBind.IsListening);
    }

    [Fact]
    public void KeyChanged_RaisesPropertyChangedForKeyAndDisplayName()
    {
        var keyBind = new KeyBind();
        var raised = new List<string?>();
        keyBind.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        keyBind.Key = Keys.Enter;

        Assert.Contains(nameof(KeyBind.Key), raised);
        Assert.Contains(nameof(KeyBind.DisplayName), raised);
    }
}
