using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت فهرست گوسفندان و جستجوی آن‌ها.
/// </summary>
public sealed class SheepViewModel(ISheepService service)
{
    private readonly ObservableCollection<Sheep> allItems = [];
    private string searchText = string.Empty;

    /// <summary>
    /// متن جستجوی شماره دام.
    /// </summary>
    public string SearchText
    {
        get => searchText;
        set
        {
            searchText = value ?? string.Empty;
            ApplyFilter();
        }
    }

    /// <summary>
    /// فهرست فیلترشده برای نمایش در جدول.
    /// </summary>
    public ObservableCollection<Sheep> FilteredItems { get; } = [];

    /// <summary>
    /// اطلاعات دام‌ها را از سرویس و در نهایت از SQL Server بارگذاری می‌کند.
    /// </summary>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        allItems.Clear();
        foreach (var sheep in await service.GetAllAsync(cancellationToken))
            allItems.Add(sheep);

        ApplyFilter();
    }

    /// <summary>
    /// فیلتر جستجو را بدون تغییر داده‌های اصلی اعمال می‌کند.
    /// </summary>
    private void ApplyFilter()
    {
        FilteredItems.Clear();

        var query = SearchText.Trim();
        var items = string.IsNullOrWhiteSpace(query)
            ? allItems
            : allItems.Where(x => x.Number.Contains(query, StringComparison.OrdinalIgnoreCase));

        foreach (var sheep in items)
            FilteredItems.Add(sheep);
    }
}
