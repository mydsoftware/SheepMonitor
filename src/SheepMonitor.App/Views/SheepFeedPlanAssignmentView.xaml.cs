using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App.Views;

public partial class SheepFeedPlanAssignmentView : UserControl
{
    public SheepFeedPlanAssignmentView() => InitializeComponent();

    private async void Sheep_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not SheepFeedPlanAssignmentViewModel vm || vm.SelectedSheep is null) return;
        try { await vm.LoadAssignmentsAsync(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private async void Assign_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SheepFeedPlanAssignmentViewModel vm) return;
        try
        {
            await vm.AssignAsync();
            MessageBox.Show("برنامه غذایی با موفقیت تخصیص داده شد.", "ثبت موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }
}
