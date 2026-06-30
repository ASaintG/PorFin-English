using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Interfaces;

public interface IReadingRepository
{
    Task<ReadingText?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReadingText>> GetByLevelAsync(CefrLevel level, int skip, int take, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<ReadingText> texts, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
