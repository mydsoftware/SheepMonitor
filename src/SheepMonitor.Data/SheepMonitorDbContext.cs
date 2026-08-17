using Microsoft.EntityFrameworkCore;

namespace SheepMonitor.Data;

/// <summary>
/// نقطه دسترسی Entity Framework Core به SQL Server.
/// </summary>
public sealed class SheepMonitorDbContext(DbContextOptions<SheepMonitorDbContext> options) : DbContext(options)
{
}
