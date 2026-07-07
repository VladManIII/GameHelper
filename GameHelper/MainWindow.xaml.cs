using CommunityToolkit.Mvvm.ComponentModel;
using GameHelper.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameHelper;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : BaseWindow
{
    public MainViewModel VievModel { get; set; } = new();

    public MainWindow()
    {
        InitializeComponent();

        AppWindow.Resize(new Windows.Graphics.SizeInt32(700, 500));
    }

    public override IViewModelLifecycle? GetViewModel() => VievModel;
}

public partial class MainViewModel : BaseViewModel
{
    public MainViewModel() { }

    [ObservableProperty]
    public partial string Status {  get; set; }

    protected override void OnActivated()
    {
        base.OnActivated();

        Status = "Ready on Activate";
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        // Handle cleanup logic here
    }

    protected override void OnVisibilityChanged()
    {
        base.OnVisibilityChanged();
        // Handle visibility change logic here
    }
}
