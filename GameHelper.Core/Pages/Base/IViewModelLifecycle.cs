using System;
using System.Collections.Generic;
using System.Text;

namespace GameHelper.Pages
{
    public interface IViewModelLifecycle
    {
        void Activated();
        void Closed();
        void VisibilityChanged();
    }
}
