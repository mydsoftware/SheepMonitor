using Microsoft.EntityFrameworkCore;
using SheepMonitor.Core.Models;

namespace SheepMonitor.Data;

/// <summary>
/// نقطه دسترسی Entity Framework Core به SQL Server.
/// </summary>
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
    public DbSet<RationPeriod> RationPeriods => Set<RationPeriod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sheep>(e => { e.ToTable("Sheep"); e.HasKey(x => x.Id); e.HasIndex(x => x.Number).IsUnique(); e.Property(x => x.Number).HasMaxLength(50).IsRequired(); e.Property(x => x.Gender).HasMaxLength(20).IsRequired(); e.Property(x => x.HealthStatus).HasMaxLength(30).IsRequired(); e.Property(x => x.InitialWeightKg).HasPrecision(8, 2); e.Property(x => x.ImagePath).HasMaxLength(500); e.Property(x => x.Notes).HasMaxLength(2000); });
        modelBuilder.Entity<WeightRecord>(e => { e.ToTable("WeightRecords"); e.HasKey(x => x.Id); e.Property(x => x.WeightKg).HasPrecision(8, 2); e.HasIndex(x => new { x.SheepId, x.WeighedAt }); e.HasOne<Sheep>().WithMany().HasForeignKey(x => x.SheepId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<HealthRecord>(e => { e.ToTable("HealthRecords"); e.HasKey(x => x.Id); e.Property(x => x.Status).HasMaxLength(30).IsRequired(); e.Property(x => x.DiseaseName).HasMaxLength(200); e.Property(x => x.Symptoms).HasMaxLength(2000); e.Property(x => x.Treatment).HasMaxLength(2000); e.Property(x => x.Notes).HasMaxLength(2000); e.HasIndex(x => new { x.SheepId, x.RecordedAt }); e.HasOne<Sheep>().WithMany().HasForeignKey(x => x.SheepId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<SheepHealthRecord>(e => { e.ToTable("SheepHealthRecords"); e.HasKey(x => x.Id); e.Property(x => x.DiseaseCode).HasMaxLength(100).IsRequired(); e.Property(x => x.SymptomsCode).HasMaxLength(100).IsRequired(); e.Property(x => x.SeverityCode).HasMaxLength(100).IsRequired(); e.Property(x => x.VeterinaryNotes).HasMaxLength(2000); e.HasIndex(x => new { x.SheepId, x.StartedAt }); e.HasOne<Sheep>().WithMany().HasForeignKey(x => x.SheepId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<SheepTreatmentRecord>(e => { e.ToTable("SheepTreatmentRecords"); e.HasKey(x => x.Id); e.Property(x => x.TreatmentCode).HasMaxLength(100).IsRequired(); e.Property(x => x.MedicineCode).HasMaxLength(100).IsRequired(); e.Property(x => x.Dose).HasPrecision(10, 3); e.Property(x => x.DoseUnitCode).HasMaxLength(100); e.Property(x => x.ResultCode).HasMaxLength(100); e.Property(x => x.Notes).HasMaxLength(2000); e.HasIndex(x => new { x.HealthRecordId, x.StartedAt }); e.HasOne<SheepHealthRecord>().WithMany().HasForeignKey(x => x.HealthRecordId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<FeedPlan>(e => { e.ToTable("FeedPlans"); e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(150).IsRequired(); e.Property(x => x.TargetGroup).HasMaxLength(100); e.Property(x => x.Notes).HasMaxLength(2000); });
        modelBuilder.Entity<FeedPlanItem>(e => { e.ToTable("FeedPlanItems"); e.HasKey(x => x.Id); e.Property(x => x.FeedName).HasMaxLength(100).IsRequired(); e.Property(x => x.AmountKgPerDay).HasPrecision(8, 3); e.Property(x => x.Unit).HasMaxLength(50).IsRequired(); e.Property(x => x.Notes).HasMaxLength(2000); e.HasOne<FeedPlan>().WithMany().HasForeignKey(x => x.FeedPlanId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<SheepFeedPlanAssignment>(e => { e.ToTable("SheepFeedPlanAssignments"); e.HasKey(x => x.Id); e.Property(x => x.Notes).HasMaxLength(2000); e.HasIndex(x => new { x.SheepId, x.StartDate }); e.HasOne<Sheep>().WithMany().HasForeignKey(x => x.SheepId).OnDelete(DeleteBehavior.Cascade); e.HasOne<FeedPlan>().WithMany().HasForeignKey(x => x.FeedPlanId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<ReferenceData>(e => { e.ToTable("ReferenceData"); e.HasKey(x => x.Id); e.Property(x => x.Category).HasMaxLength(100).IsRequired(); e.Property(x => x.Code).HasMaxLength(100).IsRequired(); e.Property(x => x.Title).HasMaxLength(200).IsRequired(); e.Property(x => x.Notes).HasMaxLength(2000); e.HasIndex(x => new { x.Category, x.Code }).IsUnique(); e.HasIndex(x => new { x.Category, x.IsActive, x.SortOrder }); });
        modelBuilder.Entity<RationCalculationRule>(e => { e.ToTable("RationCalculationRules"); e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(150).IsRequired(); e.Property(x => x.Code).HasMaxLength(100).IsRequired(); e.Property(x => x.FeedCode).HasMaxLength(100).IsRequired(); e.Property(x => x.TargetGroupCode).HasMaxLength(100); e.Property(x => x.BasePercent).HasPrecision(8, 3); e.Property(x => x.WeightCoefficient).HasPrecision(10, 5); e.Property(x => x.MinimumKg).HasPrecision(8, 3); e.Property(x => x.MaximumKg).HasPrecision(8, 3); e.Property(x => x.ProteinPercent).HasPrecision(8, 3); e.Property(x => x.EnergyPerKg).HasPrecision(10, 3); e.Property(x => x.DryMatterPercent).HasPrecision(8, 3); e.Property(x => x.Formula).HasMaxLength(1000); e.Property(x => x.Notes).HasMaxLength(2000); e.HasIndex(x => new { x.Code, x.IsActive }); });
        modelBuilder.Entity<RationPeriod>(e => { e.ToTable("RationPeriods"); e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(150).IsRequired(); e.Property(x => x.DurationDays).IsRequired(); e.Property(x => x.Notes).HasMaxLength(2000); });
    }
}
