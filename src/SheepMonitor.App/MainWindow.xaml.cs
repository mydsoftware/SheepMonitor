using System.Windows;
using SheepMonitor.App.ViewModels;
using SheepMonitor.App.Views;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await ShowDashboardAsync();
    }

    private async Task ShowDashboardAsync()
    {
        var vm = ((App)Application.Current).GetRequiredService<DashboardViewModel>();
        await vm.LoadAsync();
        ContentFrame.Content = new DashboardView { DataContext = vm };
    }

    private async void Dashboard_Click(object sender, RoutedEventArgs e) => await ShowDashboardAsync();

    /// <summary>
    /// صفحه مدیریت گوسفندان را با داده‌های واقعی دیتابیس نمایش می‌دهد.
    /// </summary>
    private async void Sheep_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<SheepViewModel>();
        await vm.LoadAsync();
        ContentFrame.Content = new SheepView { DataContext = vm };
    }

    /// <summary>
    /// گزارش رشد گوسفند انتخاب‌شده را نمایش می‌دهد.
    /// </summary>
    private async void GrowthReport_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<GrowthReportViewModel>();
        await vm.LoadAsync();
        ContentFrame.Content = new GrowthReportView { DataContext = vm };
    }

    private async void ReferenceData_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<ReferenceDataViewModel>();
        vm.Category = ReferenceDataCategories.Symptom;
        await vm.LoadAsync();
        ContentFrame.Content = new ReferenceDataView { DataContext = vm };
    }

    private async void WeightEntry_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<WeightEntryViewModel>();
        await vm.LoadAsync();
        ContentFrame.Content = new WeightEntryView { DataContext = vm };
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
        await vm.LoadAsync();
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

    private async void Ration_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<RationViewModel>();
        await vm.LoadAsync();
        ContentFrame.Content = new RationView { DataContext = vm };
    }

    private async void RationRules_Click(object sender, RoutedEventArgs e)
    {
        var vm = ((App)Application.Current).GetRequiredService<RationRuleEditorViewModel>();
        await vm.LoadAsync();
        ContentFrame.Content = new RationRuleEditorView { DataContext = vm };
    }
}
