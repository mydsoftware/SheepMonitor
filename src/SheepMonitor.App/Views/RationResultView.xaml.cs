using System.Windows;
using System.Windows.Controls;

namespace SheepMonitor.App.Views;

/// <summary>
/// رویدادهای نمایش نتیجه محاسبه جیره.
/// </summary>
public partial class RationResultView : UserControl
{
    public RationResultView() => InitializeComponent();

    private void CalculateDay_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("محاسبه روز از موتور جیره انجام می‌شود.", "جیره", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void CalculatePeriod_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("محاسبه کل دوره از موتور جیره انجام می‌شود.", "جیره", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
