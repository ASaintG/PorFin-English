using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Interfaces;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lesson>> GetByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lesson>> GetAllSortedAsync(CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<Lesson> lessons, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
