using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;

namespace SheepMonitor.Data;

public sealed class SheepMonitorDbContext(DbContextOptions<SheepMonitorDbContext> options) : DbContext(options)
{
    public DbSet<Sheep> Sheep => Set<Sheep>();
    public DbSet<WeightRecord> WeightRecords => Set<WeightRecord>();
    public DbSet<HealthRecord> HealthRecords => Set<HealthRecord>();
    public DbSet<SheepHealthRecord> SheepHealthRecords => Set<SheepHealthRecord>();
    public DbSet<SheepTreatmentRecord> SheepTreatmentRecords => Set<SheepTreatmentRecord>();
    public DbSet<FeedPlan> FeedPlans => Set<FeedPlan>();
    public DbSet<FeedPlanItem> FeedPlanItems => Set<FeedPlanItem>();
    public DbSet<SheepFeedPlanAssignment> SheepFeedPlanAssignments => Set<SheepFeedPlanAssignment>();
    public DbSet<ReferenceData> ReferenceData => Set<ReferenceData>();
    public DbSet<RationCalculationRule> RationCalculationRules => Set<RationCalculationRule>();
    public DbSet<RationMealRule> RationMealRules => Set<RationMealRule>();
    public DbSet<RationPeriod> RationPeriods => Set<RationPeriod>();
    public DbSet<FeedConsumptionRecord> FeedConsumptionRecords => Set<FeedConsumptionRecord>();
    public DbSet<FeedConsumptionItem> FeedConsumptionItems => Set<FeedConsumptionItem>();
    public DbSet<FeedPrice> FeedPrices => Set<FeedPrice>();
    public DbSet<FeedConsumptionThreshold> FeedConsumptionThresholds => Set<FeedConsumptionThreshold>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sheep>(e =>
        {
            e.ToTable("Sheep");
            e.HasKey(x => x.Id);
            e.Property(x => x.Number).HasMaxLength(100).IsRequired();
            e.Property(x => x.ImagePath).HasMaxLength(1000);
            e.Property(x => x.Gender).HasMaxLength(100).IsRequired();
            e.Property(x => x.HealthStatus).HasMaxLength(250).IsRequired();
            e.Property(x => x.InitialWeightKg).HasPrecision(10, 2);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasIndex(x => x.Number).IsUnique();
        });

        modelBuilder.Entity<WeightRecord>(e =>
        {
            e.ToTable("WeightRecords");
            e.HasKey(x => x.Id);
            e.Property(x => x.WeightKg).HasPrecision(10, 2);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasIndex(x => new { x.SheepId, x.WeighedAt });
            e.HasOne<Sheep>().WithMany().HasForeignKey(x => x.SheepId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SheepHealthRecord>(e =>
        {
            e.ToTable("SheepHealthRecords");
            e.HasKey(x => x.Id);
            e.Property(x => x.DiseaseCode).HasMaxLength(100).IsRequired();
            e.Property(x => x.SymptomsCode).HasMaxLength(100);
            e.Property(x => x.SeverityCode).HasMaxLength(100);
            e.Property(x => x.VeterinaryNotes).HasMaxLength(2000);
            e.HasIndex(x => new { x.SheepId, x.StartedAt });
            e.HasOne<Sheep>().WithMany().HasForeignKey(x => x.SheepId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SheepTreatmentRecord>(e =>
        {
            e.ToTable("SheepTreatmentRecords");
            e.HasKey(x => x.Id);
            e.Property(x => x.TreatmentCode).HasMaxLength(100).IsRequired();
            e.Property(x => x.MedicineCode).HasMaxLength(100);
            e.Property(x => x.Dose).HasPrecision(10, 3);
            e.Property(x => x.DoseUnitCode).HasMaxLength(100);
            e.Property(x => x.ResultCode).HasMaxLength(100);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasIndex(x => new { x.HealthRecordId, x.StartedAt });
            e.HasOne<SheepHealthRecord>().WithMany().HasForeignKey(x => x.HealthRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReferenceData>(e =>
        {
            e.ToTable("ReferenceData");
            e.HasKey(x => x.Id);
            e.Property(x => x.Category).HasMaxLength(100).IsRequired();
            e.Property(x => x.Code).HasMaxLength(100).IsRequired();
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasIndex(x => new { x.Category, x.Code }).IsUnique();
            e.HasIndex(x => new { x.Category, x.IsActive, x.SortOrder });
        });

        modelBuilder.Entity<FeedPrice>(e =>
        {
            e.ToTable("FeedPrices");
            e.HasKey(x => x.Id);
            e.Property(x => x.FeedCode).HasMaxLength(100).IsRequired();
            e.Property(x => x.PricePerKg).HasPrecision(18, 2);
            e.Property(x => x.Currency).HasMaxLength(10).IsRequired();
            e.HasIndex(x => new { x.FeedCode, x.EffectiveFrom });
        });

        modelBuilder.Entity<FeedConsumptionRecord>(e =>
        {
            e.ToTable("FeedConsumptionRecords");
            e.HasKey(x => x.Id);
            e.Property(x => x.FeedCode).HasMaxLength(100).IsRequired();
            e.Property(x => x.FeedTitle).HasMaxLength(250).IsRequired();
            e.Property(x => x.MealCode).HasMaxLength(100).IsRequired();
            e.Property(x => x.ActualAmountKg).HasPrecision(10, 3);
            e.Property(x => x.WasteAmountKg).HasPrecision(10, 3);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasIndex(x => new { x.ConsumedAt, x.SheepId });
            e.HasOne<Sheep>().WithMany().HasForeignKey(x => x.SheepId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<FeedConsumptionItem>(e =>
        {
            e.ToTable("FeedConsumptionItems");
            e.HasKey(x => x.Id);
            e.Property(x => x.FeedCode).HasMaxLength(100).IsRequired();
            e.Property(x => x.PlannedKg).HasPrecision(10, 3);
            e.Property(x => x.ActualKg).HasPrecision(10, 3);
            e.Property(x => x.WasteKg).HasPrecision(10, 3);
            e.HasIndex(x => new { x.FeedConsumptionRecordId, x.FeedCode });
            e.HasOne<FeedConsumptionRecord>().WithMany().HasForeignKey(x => x.FeedConsumptionRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FeedConsumptionThreshold>(e =>
        {
            e.ToTable("FeedConsumptionThresholds");
            e.HasKey(x => x.Id);
            e.Property(x => x.FeedCode).HasMaxLength(100).IsRequired();
            e.Property(x => x.LowDeviationPercent).HasPrecision(8, 2);
            e.Property(x => x.HighDeviationPercent).HasPrecision(8, 2);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasIndex(x => new { x.FeedCode, x.IsActive });
        });
    }
}
