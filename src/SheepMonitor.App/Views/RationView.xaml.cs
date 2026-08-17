using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App.Views;

public partial class RationView : UserControl
{
    public RationView() => InitializeComponent();
    private async void SaveRule_Click(object sender, RoutedEventArgs e) => await RunAsync(vm => vm.SaveRuleAsync());
    private async void Rule_SelectionChanged(object sender, SelectionChangedEventArgs e) => await RunAsync(vm => vm.LoadMealRulesAsync());
    private async void SaveMealRules_Click(object sender, RoutedEventArgs e) => await RunAsync(vm => vm.SaveMealRulesAsync());
    private async void SavePeriod_Click(object sender, RoutedEventArgs e) => await RunAsync(vm => vm.SavePeriodAsync());
    private async void CalculateDay_Click(object sender, RoutedEventArgs e) => await RunAsync(vm => vm.CalculateDayAsync());
    private async void CalculatePeriod_Click(object sender, RoutedEventArgs e) => await RunAsync(vm => vm.CalculatePeriodAsync());

    private static async Task RunAsync(Func<RationViewModel, Task> action)
    {
        if (Application.Current.MainWindow is not MainWindow window || window.ContentFrame.Content is not RationView view || view.DataContext is not RationViewModel vm) return;
        try { await action(vm); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }
}
