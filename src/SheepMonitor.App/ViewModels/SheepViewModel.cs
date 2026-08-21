using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

/// <summary>
/// مدیریت فهرست گوسفندان، جستجو و عملیات ثبت، ویرایش و حذف.
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
    /// دام انتخاب‌شده در جدول.
    /// </summary>
    public Sheep? SelectedItem { get; set; }

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
    /// گوسفند جدید را در پایگاه داده ثبت می‌کند.
    /// </summary>
    public async Task<Sheep> AddAsync(Sheep sheep, CancellationToken cancellationToken = default)
    {
        Validate(sheep);
        var result = await service.AddAsync(sheep, cancellationToken);
        allItems.Add(result);
        ApplyFilter();
        SelectedItem = result;
        return result;
    }

    /// <summary>
    /// اطلاعات گوسفند انتخاب‌شده را در پایگاه داده به‌روزرسانی می‌کند.
    /// </summary>
    public async Task UpdateAsync(Sheep sheep, CancellationToken cancellationToken = default)
    {
        Validate(sheep);
        await service.UpdateAsync(sheep, cancellationToken);
        var existing = allItems.FirstOrDefault(x => x.Id == sheep.Id);
        if (existing is not null)
        {
            var index = allItems.IndexOf(existing);
            allItems[index] = sheep;
        }

        ApplyFilter();
        SelectedItem = sheep;
    }

    /// <summary>
    /// گوسفند انتخاب‌شده را حذف می‌کند.
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await service.DeleteAsync(id, cancellationToken);
        var existing = allItems.FirstOrDefault(x => x.Id == id);
        if (existing is not null)
            allItems.Remove(existing);

        SelectedItem = null;
        ApplyFilter();
    }

    /// <summary>
    /// اعتبارسنجی اولیه اطلاعات فرم را انجام می‌دهد.
    /// </summary>
    private static void Validate(Sheep sheep)
    {
        if (string.IsNullOrWhiteSpace(sheep.Number))
            throw new ArgumentException("شماره دام الزامی است.");
        if (string.IsNullOrWhiteSpace(sheep.Gender))
            throw new ArgumentException("جنسیت دام الزامی است.");
        if (sheep.InitialWeightKg <= 0)
            throw new ArgumentException("وزن اولیه باید بیشتر از صفر باشد.");
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
