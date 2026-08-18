using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت مواد غذایی و وعده‌های اطلاعات پایه.
/// </summary>
public sealed class ReferenceDataViewModel : INotifyPropertyChanged
{
    private readonly IReferenceDataService service;
    private string category = "Feed";
    private string newTitle = string.Empty;
    private string newCode = string.Empty;
    private int newSortOrder;

    public ReferenceDataViewModel(IReferenceDataService service) => this.service = service;
    public ObservableCollection<ReferenceData> Items { get; } = [];
    public string Category { get => category; set { category = value; OnPropertyChanged(); } }
    public string NewTitle { get => newTitle; set { newTitle = value; OnPropertyChanged(); } }
    public string NewCode { get => newCode; set { newCode = value; OnPropertyChanged(); } }
    public int NewSortOrder { get => newSortOrder; set { newSortOrder = value; OnPropertyChanged(); } }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Items.Clear();
        foreach (var item in await service.GetAsync(Category, cancellationToken)) Items.Add(item);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(NewTitle) || string.IsNullOrWhiteSpace(NewCode)) return;
        var item = await service.SaveAsync(new ReferenceData { Category = Category, Code = NewCode.Trim(), Title = NewTitle.Trim(), SortOrder = NewSortOrder, IsActive = true }, cancellationToken);
        Items.Add(item);
        NewCode = string.Empty;
        NewTitle = string.Empty;
        NewSortOrder = 0;
    }

    public async Task DisableAsync(ReferenceData item, CancellationToken cancellationToken = default)
    {
        await service.DisableAsync(item.Id, cancellationToken);
        Items.Remove(item);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
