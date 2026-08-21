using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App.Views;

public partial class FeedPlanView : UserControl
{
    public FeedPlanView() => InitializeComponent();

    private async void SavePlan_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not FeedPlanViewModel vm) return;
        try
        {
            await vm.SavePlanAsync();
            MessageBox.Show("برنامه غذایی با موفقیت ایجاد شد.", "ثبت موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private async void Plan_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not FeedPlanViewModel vm || vm.SelectedPlan is null) return;
        try { await vm.LoadItemsAsync(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private async void SaveItem_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not FeedPlanViewModel vm) return;
        try
        {
            await vm.SaveItemAsync();
            MessageBox.Show("ماده غذایی با موفقیت به برنامه اضافه شد.", "ثبت موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }
}
