using System.Collections.ObjectModel;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.App.ViewModels;

public sealed class SheepViewModel(ISheepService service)
{
    public ObservableCollection<Sheep> Items { get; } = [];
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Items.Clear();
        foreach (var sheep in await service.GetAllAsync(cancellationToken)) Items.Add(sheep);
    }
}
