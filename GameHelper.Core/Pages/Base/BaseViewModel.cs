using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameHelper.Pages
{
    public abstract partial class BaseViewModel : ObservableObject, IViewModelLifecycle
    {
        public BaseViewModel() { }

        protected virtual void OnActivated() { }
        protected virtual void OnClosed() { }
        protected virtual void OnVisibilityChanged() { }

        void IViewModelLifecycle.Activated() => OnActivated();
        void IViewModelLifecycle.Closed() => OnClosed();
        void IViewModelLifecycle.VisibilityChanged() => OnVisibilityChanged();
    }
}
