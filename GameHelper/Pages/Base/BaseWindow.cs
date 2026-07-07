using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WinRT;

namespace GameHelper.Pages
{
    public abstract class BaseWindow: Window
    {
        private IViewModelLifecycle? Model => GetViewModel();

        public BaseWindow()
        {

            this.Closed += OnClosed;
            this.Activated += OnActivated;
            this.VisibilityChanged += OnVisibilityChanged;
        }

        public abstract IViewModelLifecycle? GetViewModel();

        protected void OnClosed(object sender, WindowEventArgs args)
        {
            Model?.Closed();

            this.Closed -= OnClosed;
            this.Activated -= OnActivated;
            this.VisibilityChanged -= OnVisibilityChanged;
        }

        protected void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            Model?.Activated();
        }

        protected void OnVisibilityChanged(object sender, WindowVisibilityChangedEventArgs args)
        {
            Model?.VisibilityChanged();
        }

    }
}
