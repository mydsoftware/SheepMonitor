using System.IO;

namespace SheepMonitor.App.Services;

public sealed class ImageStorageService
{
    public async Task<string> SaveAsync(string sourcePath, int sheepId, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(sourcePath)) throw new FileNotFoundException("فایل تصویر پیدا نشد.", sourcePath);
        var root = Path.Combine(AppContext.BaseDirectory, "SheepImages");
        Directory.CreateDirectory(root);
        var extension = Path.GetExtension(sourcePath);
        var destination = Path.Combine(root, $"{sheepId}{extension}");
        await using var source = File.OpenRead(sourcePath);
        await using var target = File.Create(destination);
        await source.CopyToAsync(target, cancellationToken);
        return destination;
    }
}
