using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;
using SheepMonitor.Core.Models;

namespace SheepMonitor.App.Views;

/// <summary>
/// رویدادهای فرم ویرایش قوانین و وعده‌های جیره.
/// </summary>
public partial class RationRuleEditorView : UserControl
{
    public RationRuleEditorView() => InitializeComponent();

    private async void Rule_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is RationRuleEditorViewModel vm) await vm.LoadMealsAsync();
    }

    private async void SaveRule_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is RationRuleEditorViewModel vm)
        {
            try { await vm.SaveRuleAsync(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }
    }

    private async void SaveMeal_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not RationRuleEditorViewModel vm || MealsGrid.SelectedItem is not RationMealRule meal) return;
        try { await vm.SaveMealAsync(meal); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }
}
