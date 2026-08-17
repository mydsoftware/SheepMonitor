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

    private async void Treatment_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = ((App)Application.Current).GetRequiredService<TreatmentViewModel>();
        if (ContentFrame.Content is HealthView healthView && healthView.DataContext is HealthViewModel healthVm && healthVm.SelectedSheep is not null)
        {
            await viewModel.LoadAsync(healthVm.SelectedSheep.Id);
        }
        else
        {
            var sheep = await ((App)Application.Current).GetRequiredService<ISheepService>().GetAllAsync();
            if (sheep.Count == 0) { MessageBox.Show("ابتدا حداقل یک گوسفند ثبت کنید.", "اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            await viewModel.LoadAsync(sheep[0].Id);
        }
        ContentFrame.Content = new TreatmentView { DataContext = viewModel };
    }
}
