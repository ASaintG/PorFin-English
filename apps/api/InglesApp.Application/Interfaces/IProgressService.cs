using InglesApp.Application.DTOs.Progress;

namespace InglesApp.Application.Interfaces;

public interface IProgressService
{
    Task<ProgressStatsDto> GetStatsAsync(string userId, CancellationToken cancellationToken = default);
    Task UpdateStreakAsync(string userId, CancellationToken cancellationToken = default);
    Task AddPracticeMinutesAsync(string userId, int minutes, CancellationToken cancellationToken = default);
}
