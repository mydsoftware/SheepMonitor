using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SheepMonitor.Data;

public sealed class DesignTimeSheepMonitorDbContextFactory : IDesignTimeDbContextFactory<SheepMonitorDbContext>
{
    public SheepMonitorDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SheepMonitorDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SheepMonitor;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new SheepMonitorDbContext(options);
    }
}
