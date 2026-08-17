using System.Windows;
using SheepMonitor.App.Views;
using SheepMonitor.App.ViewModels;

namespace SheepMonitor.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void ReferenceData_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = ((App)Application.Current).GetRequiredService<ReferenceDataViewModel>();
        viewModel.Category = Core.Services.ReferenceDataCategories.Symptom;
        await viewModel.LoadAsync();
        ContentFrame.Content = new ReferenceDataView { DataContext = viewModel };
    }

    private async void Health_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = ((App)Application.Current).GetRequiredService<HealthViewModel>();
        await viewModel.LoadAsync();
        ContentFrame.Content = new HealthView { DataContext = viewModel };
    }
}
