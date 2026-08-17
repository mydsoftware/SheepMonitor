using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

public sealed class ReferenceDataViewModel : INotifyPropertyChanged
{
    private readonly IReferenceDataService _service;
    private string _category = string.Empty;
    private string _newTitle = string.Empty;
    private string _newCode = string.Empty;

    public ReferenceDataViewModel(IReferenceDataService service) => _service = service;

    public ObservableCollection<ReferenceData> Items { get; } = [];
    public string Category { get => _category; set { _category = value; OnPropertyChanged(); } }
    public string NewTitle { get => _newTitle; set { _newTitle = value; OnPropertyChanged(); } }
    public string NewCode { get => _newCode; set { _newCode = value; OnPropertyChanged(); } }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Items.Clear();
        foreach (var item in await _service.GetAsync(Category, cancellationToken)) Items.Add(item);
    }

    public async Task AddAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(NewTitle) || string.IsNullOrWhiteSpace(NewCode)) return;
        var item = await _service.AddAsync(new ReferenceData { Category = Category, Code = NewCode.Trim(), Title = NewTitle.Trim() }, cancellationToken);
        Items.Add(item);
        NewCode = string.Empty;
        NewTitle = string.Empty;
    }

    public async Task DisableAsync(ReferenceData item, CancellationToken cancellationToken = default)
    {
        await _service.DisableAsync(item.Id, cancellationToken);
        Items.Remove(item);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
