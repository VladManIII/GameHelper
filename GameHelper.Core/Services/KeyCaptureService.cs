using System;
using System.Windows.Forms;

using HookLib;
using HookLib.Models;

namespace GameHelper.Services;

// Single choke point for "listen for the next global keypress" so Models (e.g. KeyBind)
// don't have to reach into HookLib's static Controllers singleton directly.
public static class KeyCaptureService
{
    public static IDisposable ListenOnce(Action<Keys> onCaptured)
    {
        KeySubscription? subscription = null;

        void Handler(Keys key, KeysState state)
        {
            if (state != KeysState.Down) return;

            subscription?.Dispose();
            onCaptured(key);
        }

        subscription = new KeySubscription(Handler);
        Controllers.KeyboardHookController.KeyChanged += Handler;
        return subscription;
    }

    private sealed class KeySubscription : IDisposable
    {
        private Action<Keys, KeysState>? _handler;

        public KeySubscription(Action<Keys, KeysState> handler) => _handler = handler;

        public void Dispose()
        {
            if (_handler is null) return;

            Controllers.KeyboardHookController.KeyChanged -= _handler;
            _handler = null;
        }
    }
}
