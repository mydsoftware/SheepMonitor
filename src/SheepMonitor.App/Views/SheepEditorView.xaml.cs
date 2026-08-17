using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App.Views;

public partial class SheepEditorView : UserControl
{
    public SheepEditorView() => InitializeComponent();

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SheepEditorViewModel vm) return;
        try
        {
            await vm.SaveAsync();
            MessageBox.Show("گوسفند با موفقیت ثبت شد.", "ثبت موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
