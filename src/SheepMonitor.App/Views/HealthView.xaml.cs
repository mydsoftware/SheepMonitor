using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App.Views;

public partial class HealthView : UserControl
{
    public HealthView() => InitializeComponent();

    private async void Load_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not HealthViewModel vm) return;
        try
        {
            await vm.LoadRecordsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not HealthViewModel vm) return;
        try
        {
            await vm.SaveAsync();
            MessageBox.Show("سابقه بیماری با موفقیت ثبت شد.", "ثبت موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void Recover_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not HealthViewModel vm) return;
        try
        {
            await vm.MarkSelectedRecoveredAsync();
            MessageBox.Show("وضعیت بهبودی ثبت شد.", "ثبت موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
