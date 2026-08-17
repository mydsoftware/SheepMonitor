using SheepMonitor.Core.Models;

namespace SheepMonitor.Core.Services;

/// <summary>
/// قرارداد مدیریت سوابق درمان گوسفندان.
/// </summary>
public interface ITreatmentService
{
    Task<IReadOnlyList<SheepTreatmentRecord>> GetByHealthRecordAsync(int healthRecordId, CancellationToken cancellationToken = default);
    Task<SheepTreatmentRecord> AddAsync(SheepTreatmentRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(SheepTreatmentRecord record, CancellationToken cancellationToken = default);
}
