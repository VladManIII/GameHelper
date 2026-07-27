using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GameHelper.Views;

public sealed partial class GamesPage : Page
{
    private MainViewModel? ViewModel => DataContext as MainViewModel;

    public GamesPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is MainViewModel viewModel)
            DataContext = viewModel;
    }

    private void MacroField_GotFocus(object sender, RoutedEventArgs e) => ViewModel?.BeginFieldEdit();

    private void MacroField_LostFocus(object sender, RoutedEventArgs e) => ViewModel?.EndFieldEdit();
}
