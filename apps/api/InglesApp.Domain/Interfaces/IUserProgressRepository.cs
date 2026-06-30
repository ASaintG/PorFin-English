using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Interfaces;

public interface IUserProgressRepository
{
    Task<UserProgress?> GetByUserAndItemAsync(string userId, string itemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserProgress>> GetDueItemsAsync(string userId, DateTime asOf, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserProgress>> GetDueByItemTypeAsync(string userId, ItemType itemType, DateTime asOf, CancellationToken cancellationToken = default);
    Task<UserProgress> UpsertAsync(UserProgress progress, CancellationToken cancellationToken = default);
    Task<long> CountMasteredAsync(string userId, int minRepetitions = 3, CancellationToken cancellationToken = default);
    Task<long> CountMasteredByItemTypeAsync(string userId, ItemType itemType, int minRepetitions = 3, CancellationToken cancellationToken = default);
    Task<long> CountDueAsync(string userId, DateTime asOf, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserProgress>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
}
