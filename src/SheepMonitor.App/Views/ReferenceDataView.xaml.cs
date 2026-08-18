using System.Windows;
using System.Windows.Controls;
using SheepMonitor.App.ViewModels;
using SheepMonitor.Core.Models;

namespace SheepMonitor.App.Views;

/// <summary>
/// کنترل مدیریت مواد غذایی و وعده‌های اطلاعات پایه.
/// </summary>
public partial class ReferenceDataView : UserControl
{
    private readonly ReferenceDataViewModel viewModel;

    public ReferenceDataView()
    {
        InitializeComponent();
        viewModel = ((App)Application.Current).GetRequiredService<ReferenceDataViewModel>();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.LoadAsync();
    }

    private async void Category_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized || CategoryBox.SelectedValue is not string category) return;
        viewModel.Category = category;
        await viewModel.LoadAsync();
    }

    private async void Save_Click(object sender, RoutedEventArgs e) => await viewModel.SaveAsync();

    private async void Update_Click(object sender, RoutedEventArgs e) => await viewModel.UpdateSelectedAsync();

    private void CancelEdit_Click(object sender, RoutedEventArgs e) => viewModel.ClearEditor();

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: ReferenceData item }) viewModel.BeginEdit(item);
    }

    private async void Disable_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: ReferenceData item }) await viewModel.DisableAsync(item);
    }
}
