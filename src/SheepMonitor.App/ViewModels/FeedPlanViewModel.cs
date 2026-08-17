using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت برنامه‌های غذایی و اقلام هر برنامه.
/// </summary>
public sealed class FeedPlanViewModel(IFeedPlanService feedPlanService, IReferenceDataService referenceDataService)
{
    public ObservableCollection<FeedPlan> Plans { get; } = [];
    public ObservableCollection<FeedPlanItem> Items { get; } = [];
    public IReadOnlyList<ReferenceData> Feeds { get; private set; } = [];
    public FeedPlan? SelectedPlan { get; set; }
    public FeedPlan PlanModel { get; } = new() { StartDate = DateTime.Today, IsActive = true };
    public FeedPlanItem ItemModel { get; } = new();

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Plans.Clear();
        foreach (var plan in await feedPlanService.GetAllAsync(cancellationToken)) Plans.Add(plan);
        Feeds = await referenceDataService.GetAsync("ماده غذایی", cancellationToken);
    }

    public async Task LoadItemsAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedPlan is null) throw new InvalidOperationException("ابتدا یک برنامه غذایی را انتخاب کنید.");
        Items.Clear();
        foreach (var item in await feedPlanService.GetItemsAsync(SelectedPlan.Id, cancellationToken)) Items.Add(item);
    }

    public async Task SavePlanAsync(CancellationToken cancellationToken = default)
    {
        var saved = await feedPlanService.AddAsync(PlanModel, cancellationToken);
        Plans.Add(saved);
        SelectedPlan = saved;
    }

    public async Task SaveItemAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedPlan is null) throw new InvalidOperationException("ابتدا یک برنامه غذایی را انتخاب کنید.");
        ItemModel.FeedPlanId = SelectedPlan.Id;
        await feedPlanService.AddItemAsync(ItemModel, cancellationToken);
        await LoadItemsAsync(cancellationToken);
    }
}
