using System.Windows;
using SheepMonitor.App.Views;
using SheepMonitor.App.ViewModels;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private async void ReferenceData_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<ReferenceDataViewModel>();
        vm.Category = ReferenceDataCategories.Symptom;
        await vm.LoadAsync();
        ContentFrame.Content = new ReferenceDataView { DataContext = vm };
    }

    private async void Health_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<HealthViewModel>();
        await vm.LoadAsync();
        ContentFrame.Content = new HealthView { DataContext = vm };
    }

    private async void Treatment_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<TreatmentViewModel>();
        var sheep = await ((App)Application.Current).GetRequiredService<ISheepService>().GetAllAsync();
        if (sheep.Count == 0) { MessageBox.Show("ابتدا حداقل یک گوسفند ثبت کنید.", "اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information); return; }
        await vm.LoadAsync(sheep[0].Id);
        ContentFrame.Content = new TreatmentView { DataContext = vm };
    }

    private async void FeedPlan_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<FeedPlanViewModel>();
        await vm.LoadAsync();
        ContentFrame.Content = new FeedPlanView { DataContext = vm };
    }

    private async void FeedAssignment_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<SheepFeedPlanAssignmentViewModel>();
        await vm.LoadAsync();
        ContentFrame.Content = new SheepFeedPlanAssignmentView { DataContext = vm };
    }
}
