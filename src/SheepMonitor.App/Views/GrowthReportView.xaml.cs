using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App.Views;

public partial class GrowthReportView : UserControl
{
    public GrowthReportView() => InitializeComponent();

    private async void Report_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not GrowthReportViewModel vm) return;
        try
        {
            await vm.LoadReportAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
