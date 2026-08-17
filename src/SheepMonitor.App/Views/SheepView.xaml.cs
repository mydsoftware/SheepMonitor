using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App.Views;

public partial class SheepView : UserControl
{
    public SheepView() => InitializeComponent();

    private async void Load_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is SheepViewModel vm)
            await vm.LoadAsync();
    }
}
