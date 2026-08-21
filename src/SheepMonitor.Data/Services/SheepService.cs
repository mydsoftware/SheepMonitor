using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;
using SheepMonitor.Core.Services;

namespace SheepMonitor.Data.Services;

public sealed class SheepService(SheepMonitorDbContext db) : ISheepService
{
    public async Task<IReadOnlyList<Sheep>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Sheep.AsNoTracking().OrderBy(x => x.Number).ToListAsync(cancellationToken);

    public Task<Sheep?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        db.Sheep.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Sheep> AddAsync(Sheep sheep, CancellationToken cancellationToken = default)
    {
        Validate(sheep);

        var duplicate = await db.Sheep.AnyAsync(x => x.Number == sheep.Number, cancellationToken);
        if (duplicate)
            throw new InvalidOperationException("شماره دام تکراری است.");

        db.Sheep.Add(sheep);
        await db.SaveChangesAsync(cancellationToken);
        return sheep;
    }

    public async Task UpdateAsync(Sheep sheep, CancellationToken cancellationToken = default)
    {
        Validate(sheep);

        var duplicate = await db.Sheep.AnyAsync(
            x => x.Id != sheep.Id && x.Number == sheep.Number,
            cancellationToken);
        if (duplicate)
            throw new InvalidOperationException("شماره دام تکراری است.");

        db.Sheep.Update(sheep);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sheep = await db.Sheep.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (sheep is null)
            return;

        db.Sheep.Remove(sheep);
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// اعتبارسنجی اطلاعات پایه دام را در لایه داده انجام می‌دهد.
    /// </summary>
    private static void Validate(Sheep sheep)
    {
        if (string.IsNullOrWhiteSpace(sheep.Number))
            throw new InvalidOperationException("شماره دام الزامی است.");

        if (sheep.InitialWeightKg <= 0)
            throw new InvalidOperationException("وزن اولیه باید بیشتر از صفر باشد.");
    }
}
