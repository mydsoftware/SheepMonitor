using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App.Views;

public partial class TreatmentView : UserControl
{
    public TreatmentView() => InitializeComponent();

    private async void LoadHealth_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not TreatmentViewModel vm) return;
        try { await vm.LoadHealthRecordsAsync(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private async void Load_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not TreatmentViewModel vm) return;
        try { await vm.LoadTreatmentsAsync(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not TreatmentViewModel vm) return;
        try
        {
            await vm.SaveAsync();
            MessageBox.Show("درمان با موفقیت ثبت شد.", "ثبت موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }
}
