using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت تخصیص برنامه غذایی به گوسفندان.
/// </summary>
public sealed class SheepFeedPlanAssignmentViewModel(ISheepService sheepService, IFeedPlanService feedPlanService, ISheepFeedPlanService assignmentService)
{
    public ObservableCollection<Sheep> Sheep { get; } = [];
    public ObservableCollection<FeedPlan> Plans { get; } = [];
    public ObservableCollection<SheepFeedPlanAssignment> Assignments { get; } = [];
    public Sheep? SelectedSheep { get; set; }
    public FeedPlan? SelectedPlan { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public SheepFeedPlanAssignment? ActiveAssignment { get; private set; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Sheep.Clear();
        Plans.Clear();
        foreach (var sheep in await sheepService.GetAllAsync(cancellationToken)) Sheep.Add(sheep);
        foreach (var plan in await feedPlanService.GetAllAsync(cancellationToken)) Plans.Add(plan);
    }

    public async Task LoadAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedSheep is null) throw new InvalidOperationException("ابتدا یک گوسفند را انتخاب کنید.");
        Assignments.Clear();
        foreach (var item in await assignmentService.GetBySheepAsync(SelectedSheep.Id, cancellationToken)) Assignments.Add(item);
        ActiveAssignment = await assignmentService.GetActiveAsync(SelectedSheep.Id, cancellationToken);
    }

    public async Task AssignAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedSheep is null) throw new InvalidOperationException("ابتدا یک گوسفند را انتخاب کنید.");
        if (SelectedPlan is null) throw new InvalidOperationException("ابتدا یک برنامه غذایی را انتخاب کنید.");
        var assignment = new SheepFeedPlanAssignment
        {
            SheepId = SelectedSheep.Id,
            FeedPlanId = SelectedPlan.Id,
            StartDate = StartDate,
            EndDate = EndDate,
            IsActive = true,
            Notes = Notes
        };
        await assignmentService.AssignAsync(assignment, cancellationToken);
        await LoadAssignmentsAsync(cancellationToken);
    }

    public async Task EndActiveAsync(DateTime endDate, CancellationToken cancellationToken = default)
    {
        if (ActiveAssignment is null) throw new InvalidOperationException("برنامه فعال وجود ندارد.");
        await assignmentService.EndAsync(ActiveAssignment.Id, endDate, cancellationToken);
        await LoadAssignmentsAsync(cancellationToken);
    }
}
