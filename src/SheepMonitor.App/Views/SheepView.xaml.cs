using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;
using SheepMonitor.Core.Models;

namespace SheepMonitor.App.Views;

public partial class SheepView : UserControl
{
    public SheepView() => InitializeComponent();

    private async void Load_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is SheepViewModel vm)
            await vm.LoadAsync();
    }

    private async void New_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SheepViewModel vm)
            return;

        var sheep = new Sheep
        {
            Number = "",
            Gender = "",
            InitialWeighingDate = DateTime.Today
        };

        try
        {
            await vm.AddAsync(sheep);
            MessageBox.Show("گوسفند جدید با موفقیت ثبت شد.", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "خطا در ثبت گوسفند", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void Edit_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SheepViewModel vm || vm.SelectedItem is null)
        {
            MessageBox.Show("ابتدا یک گوسفند را انتخاب کنید.", "هشدار", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await vm.UpdateAsync(vm.SelectedItem);
            MessageBox.Show("اطلاعات گوسفند با موفقیت به‌روزرسانی شد.", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "خطا در ویرایش گوسفند", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SheepViewModel vm || vm.SelectedItem is null)
        {
            MessageBox.Show("ابتدا یک گوسفند را انتخاب کنید.", "هشدار", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"آیا از حذف گوسفند شماره {vm.SelectedItem.Number} مطمئن هستید؟",
            "تأیید حذف",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            await vm.DeleteAsync(vm.SelectedItem.Id);
            MessageBox.Show("گوسفند با موفقیت حذف شد.", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "خطا در حذف گوسفند", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
