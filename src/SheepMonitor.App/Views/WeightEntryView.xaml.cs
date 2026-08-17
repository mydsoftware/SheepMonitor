using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App.Views;

public partial class WeightEntryView : UserControl
{
    public WeightEntryView() => InitializeComponent();

    private async void Load_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is WeightEntryViewModel vm) await vm.LoadAsync();
    }

    private async void Calculate_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is WeightEntryViewModel vm)
        {
            try { await vm.CalculateAsync(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is WeightEntryViewModel vm)
        {
            try
            {
                await vm.SaveAsync();
                MessageBox.Show("وزن‌ها با موفقیت ثبت شدند.", "ثبت موفق", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }
    }
}
